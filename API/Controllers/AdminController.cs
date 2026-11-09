using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager, IUnitOfWork uow) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users.ToListAsync();
        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userList.Add(new // anonymous object - fine to use for small use case like this but we loose type safety 
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles.ToList()
            })  ; 
        }

        return Ok(userList);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{userId}")]
    public async Task<ActionResult<IList<string>>> EditRoles(string userId, [FromQuery] string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must provide at least 1 role");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByIdAsync(userId);

        if(user == null) return BadRequest("Could not retrive user");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForModeration(int pageNumber, int pageSize)
    {
        var photos = await uow.PhotoRepository.GetUnapprovedPhotos(pageNumber, pageSize);
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await uow.PhotoRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Photo could not be found");
        photo.IsApproved = true;

        // this is the first image approved for member so set it to the member's 
        // 'profile' image
        var member = await uow.MemberRepository.GetMemberByPhotoId(photoId);

        if (member == null) return BadRequest("No member for photo found");

        if (member.ImageUrl == null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
            uow.MemberRepository.Update(member);
        }

        uow.PhotoRepository.Update(photo); // not strictly needed, method below already checks to see if there are changes before commit them 
        await uow.Complete();
        return Ok();
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await uow.PhotoRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Photo could not be found");
        photo.IsApproved = false;
        uow.PhotoRepository.Update(photo);
        await uow.Complete();
        return Ok();
    }
}
