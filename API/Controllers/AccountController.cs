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
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto regsiterDto)
    // public async Task<ActionResult<AppUser>> Register(string email, string displayName, string password)
    // by convention if parameters are simple types, a query string is used to match the parameters - the body is not checked
    // this is a result of the [ApiController] attribute on BaseApiController 
    // generally this is bad because URLs typically get logged by logging systems, monitoring tools etc
    // its better to include confidential information in the body and is why RequsetDto was created
    {
        // hash the password with a salt (randomly generated key provided by this class library)
        // Rule of thumb: if a type implements IDisposable, wrap it in a 'using' - this is about
        // deterministic resource cleanup, not memory management (the GC still reclaims the object's memory later)
        // using var hmac = new HMACSHA512(); // Dispose() is called when the method exits, releasing native
                                           // crypto handles immediately (and clearing the key) rather than
                                           // waiting for the GC/finalizer to do it at some unknown later point

        var user = new AppUser
        {
            DisplayName = regsiterDto.DisplayName,
            Email = regsiterDto.Email,
            UserName = regsiterDto.Email,
            // PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regsiterDo.Password)),
            // PasswordSalt = hmac.Key, // a new Key is generated for each new instance of a HMAC
            Member = new Member
            {
                DisplayName = regsiterDto.DisplayName,
                Gender = regsiterDto.Gender,
                Country = regsiterDto.Country,
                City = regsiterDto.City,
                DateOfBirth = regsiterDto.DateOfBirth
            }
        };

        var result = await userManager.CreateAsync(user, regsiterDto.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }

            return ValidationProblem();
        }

        await userManager.AddToRoleAsync(user, "Member");
        
        return await user.ToDto(tokenService);
    }

    [HttpPost("login")]
    // no type safety with IActionResult so typically not used where type safety is desired
    // public async Task<IActionResult> Login() {}

    // non simple types are bound to request body but we can add the [FromQuery] attribute to 
    // the parameter to give ApiController hint to fetch it from query string instead 
    //  public async Task<ActionResult<AppUser>> Login([FromQuery]LoginDto loginDto)
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);

        if (user == null ) return Unauthorized("Invalid email address");


        // all this is manual and taken care of by ASP.NET Identity
        // using var hmac = new HMACSHA512(user.PasswordSalt); // use same salt we stored when the user was registered to be able to get the correct hash for password provided
        // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        // }

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password); // identity's job to resolve this 

        if (!result) return Unauthorized("Invalid password");

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);
        
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null) return NoContent();

        var user = await userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null) return Unauthorized();

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);
    }

    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // not accessible from any JS including our js on client side,
            Secure = true, // will not be sent over http only https
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7) // long lived cookie
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
