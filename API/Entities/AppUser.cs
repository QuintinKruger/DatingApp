using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser
{

    // by Entity Framework convention, this will be the PK  (because it is named Id)
    // public string Id { get; set; } = Guid.NewGuid().ToString();
    // above is replaced by properties in IdentityUser

    // required forces instances to provide this property when the object is intialized
    // this was introduced in .NET 7
    public required string DisplayName { get; set; }

    // public required string Email { get; set; }
    // above is replaced by properties in IdentityUser

    public string? ImageUrl { get; set; }

    // anonimity of password that can still be unshashed by hackers - hence salt
    // public required byte[] PasswordHash { get; set; }
    // // randomizes PasswordHash for added security - altough there is still some vulnerability as this can also be used to 
    // // rework password
    // public required byte[] PasswordSalt { get; set; }
    // above is replaced by properties in IdentityUser

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation property 
    public Member Member { get; set; } = null!;
    

}
