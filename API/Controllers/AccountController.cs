using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Extensions;

namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto regsiterDo)
    // public async Task<ActionResult<AppUser>> Register(string email, string displayName, string password)
    // by convention if parameters are simple types, a query string is used to match the parameters - the body is not checked
    // this is a result of the [ApiController] attribute on BaseApiController 
    // generally this is bad because URLs typically get logged by logging systems, monitoring tools etc
    // its better to include confidential information in the body and is why RequsetDto was created
    {
        if (await EmailExists(regsiterDo.Email)) return BadRequest("Email taken");

        // hash the password with a salt (randomly generated key provided by this class library)
        // Rule of thumb: if a type implements IDisposable, wrap it in a 'using' - this is about
        // deterministic resource cleanup, not memory management (the GC still reclaims the object's memory later)
        using var hmac = new HMACSHA512(); // Dispose() is called when the method exits, releasing native
                                           // crypto handles immediately (and clearing the key) rather than
                                           // waiting for the GC/finalizer to do it at some unknown later point

        var user = new AppUser
        {
            DisplayName = regsiterDo.DisplayName,
            Email = regsiterDo.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regsiterDo.Password)),
            PasswordSalt = hmac.Key, // a new Key is generated for each new instance of a HMAC
            Member = new Member
            {
                DisplayName = regsiterDo.DisplayName,
                Gender = regsiterDo.Gender,
                Country = regsiterDo.Country,
                City = regsiterDo.City,
                DateOfBirth = regsiterDo.DateOfBirth
            }
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(); // this is the async provided method to call to asynchrnously save to the DB 
        
        return user.ToDto(tokenService);
    }

    [HttpPost("login")]
    // no type safety with IActionResult so typically not used where type safety is desired
    // public async Task<IActionResult> Login() {}

    // non simple types are bound to request body but we can add the [FromQuery] attribute to 
    // the parameter to give ApiController hint to fetch it from query string instead 
    //  public async Task<ActionResult<AppUser>> Login([FromQuery]LoginDto loginDto)
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(user => user.Email == loginDto.Email);
        if (user == null ) return Unauthorized("Invalid email address");

        using var hmac = new HMACSHA512(user.PasswordSalt); // use same salt we stored when the user was registered to be able to get the correct hash for password provided
        
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return user.ToDto(tokenService);
        
    }

    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(user => user.Email.ToLower()  == email.ToLower());
    }

}
