using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

// IConfiguration is a service registered with .NET we can use to access
// values defined in appsettings.json (or equivalent env specific config)
public class TokenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("No TokenKey found in config."); // double coalescing key try get token key and if null do what ever is defined on the right of ??
        if (tokenKey.Length < 64 ) throw new Exception("Token key needs to be 64 >= 64 characters"); // requirement of hmac used
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)); // tokenKey is used to sign the JWT token (produces the signature portion of JWT token)
                                                                              // symetric security means private key is used to encrypt and decrypt signature
                                                                              // key does not leave our app thats why this is fine
        // a claim is what a user claims about themselves 
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id)
            // we can define custom claims types too 
            // new ("CustomClaimTypeHere", "CustomValue here")
        };

        var roles = await userManager.GetRolesAsync(user);
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // properties the token needs
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        // long lived token to be stored as HTTP Only cookie on client browser
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}
