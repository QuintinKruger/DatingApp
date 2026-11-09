
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Interfaces;
using API.Helpers;

// naming convention <plural>Controller
namespace API.Controllers;

// dependency injection of the AppDbContext because it was registered with services in Program.cs
[Authorize] // all actions need to be authorized to be used in this controller
public class MembersController(
    IUnitOfWork uow,
    IPhotoService photoService) : BaseApiController
{
    [HttpGet]
    // best practice to handle requests asynchronously to allow other requests to still be processed 
    // while the current request is waiting for response from DB or similar async-able invocation 
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery]MemberParams memberParams)
    {
        // await the async call means to make the call and hand it to a thread to handle the response once 
        // it comes back. This unblocks the current thread servicing the request received to receive other requests
        // while this call is still being made

        memberParams.CurrentMemberId = User.GetMemberId();
        
        var members = await uow.MemberRepository.GetMembersAsync(memberParams);
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> GetMember(string id)
    {
        var member = await uow.MemberRepository.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();
        return member;
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
    {
        if (User.GetMemberId() == id)
        {
             return Ok(await uow.MemberRepository.GetPhotosForMemberAsync(id, false));
        }
        return Ok(await uow.MemberRepository.GetPhotosForMemberAsync(id, true));
    }

    [HttpPut]
    // response returns with no item in body 
    public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
    {
        // User is a property available to us from ConctrollerBase class (grandparent of this class)
        var memberId = User.GetMemberId();

        var member = await uow.MemberRepository.GetMemberForUpdatesAsync(memberId);

        if(member == null) return BadRequest("No member for requested id");

        member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
        member.Description = memberUpdateDto.Description ?? member.Description;
        member.Country = memberUpdateDto.Country ?? member.Country;
        member.City = memberUpdateDto.City ?? member.City;

        // also need to update the navigation property of a member 
        member.User.DisplayName =  memberUpdateDto.DisplayName ?? member.User.DisplayName;

        uow.MemberRepository.Update(member); // optional - simply prevents fall through to BadRequest below

        if (await uow.Complete()) return NoContent();

        return BadRequest("Failed to update member");
        
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file) // get the file from the form of http request
    {
        var member = await uow.MemberRepository.GetMemberForUpdatesAsync(User.GetMemberId());
        if (member == null) return BadRequest("Cannot update member");

        var result = await photoService.UploadPhotoAsync(file);

        if (result.Error != null ) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            MemberId = User.GetMemberId()
        };

        member.Photos.Add(photo);

        if (await uow.Complete()) return photo;

        return BadRequest("Problem uploading photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
         var member = await uow.MemberRepository.GetMemberForUpdatesAsync(User.GetMemberId());

         if (member == null) return BadRequest("Cannot get member from token");

         var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

         if(member.ImageUrl == photo?.Url || photo == null) return BadRequest("Cannot set this as main image");

         member.ImageUrl = photo.Url;
         member.User.ImageUrl = photo.Url;

         if (await uow.Complete()) return NoContent();

         return BadRequest("Failed to update main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var member = await uow.MemberRepository.GetMemberForUpdatesAsync(User.GetMemberId());

         if (member == null) return BadRequest("Cannot get member from token");

         var photo = await uow.PhotoRepository.GetPhotoById(photoId);

         if (photo == null || photo.Url == member.ImageUrl) return BadRequest("This photo cannot be deleted");

         if(photo.PublicId != null )
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);
        }

        member.Photos.Remove(photo);

        if (await uow.Complete()) return Ok();

        return BadRequest("Problem deleting the photo");

    }



}

