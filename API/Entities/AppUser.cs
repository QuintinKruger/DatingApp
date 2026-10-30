namespace API.Entities;

public class AppUser
{

    // by Entity Framework convention, this will be the PK  (because it is named Id)
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // required forces instances to provide this property when the object is intialized
    // this was introduced in .NET 7
    public required string DisplayName { get; set; }

    public required string Email { get; set; }

    public string? ImageUrl { get; set; }

    // anonimity of password that can still be unshashed by hackers - hence salt
    public required byte[] PasswordHash { get; set; }

    // randomizes PasswordHash for added security - altough there is still some vulnerability as this can also be used to 
    // rework password
    public required byte[] PasswordSalt { get; set; }

    // Navigation property 
    public Member Member { get; set; } = null!;
    

}
