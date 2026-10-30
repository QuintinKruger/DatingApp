
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Member
{
    public string Id { get; set; } = null!; // null forgiving operator because nullable is true is csproj file, tells compiler to silence the warning (has no bearing on final code)
    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string DisplayName { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }

    // Known as navigation property in Entity Framework
    // we will be able to navigate from our Member object to the 
    // user because they are related
    // Property cannot be required EntityFramework has a problem with this
    // as it first needs to perform DB lookup before being able to set the value
    [ForeignKey(nameof(Id))] // provide foreign key that defines the relationship
    [JsonIgnore] // because this class is used as a DTO object too, we dont return this in json response
    public AppUser User { get; set; } = null!;

    [JsonIgnore]
    public List<Photo> Photos {get;set;} = [];
}
