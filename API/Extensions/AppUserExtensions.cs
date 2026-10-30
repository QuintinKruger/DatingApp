using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

// extensions are a C# feature NOT a framework feature 
public static class AppUserExtensions
{
    // structure of an extenstion method 
    // Note - this is a static method thus we cannot access tokenService via dependency injection
    public static UserDto ToDto(this AppUser appUser, ITokenService tokenService) // first argument must be the type that this method extends, denoting this to the compiler using the 'this' modifier
                                                                                  // this simply compiles down to an ordinary static method call but allows us to call the method like this - user.ToDto(tokenService), where use is the instance 
                                                                                  // of AppUser to be extended 
    {
        return new UserDto
        {
            Id = appUser.Id,
            DisplayName = appUser.DisplayName,
            Email = appUser.Email,
            Token = tokenService.CreateToken(appUser),
            ImageUrl = appUser.ImageUrl
        };
    }
}
