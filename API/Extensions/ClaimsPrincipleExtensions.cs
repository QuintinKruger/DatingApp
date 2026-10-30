using System;
using System.Security.Claims;

namespace API.Entities;

public static class ClaimsPrincipleExtensions
{
    public static string GetMemberId(this ClaimsPrincipal user)
    {
        // we can get the member ID from the token - recall we added the claim type 
        // to the token when we created the token NameIdentifier. The value of this claim
        // was the memberId
        // User is a property available to us from ConctrollerBase class (grandparent of this class)
        return user.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new Exception("Cannot get memberId from token"); 
    }
}
