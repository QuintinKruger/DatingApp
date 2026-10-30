using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{

    // validation attributes
    // although required modifier can be defined, this attribute provides better user 
    // responses that sticking with required keyword
    [Required]
     [EmailAddress]
    public string Email { get; set; } = "";
    [Required]
    public required string  DisplayName { get; set; } = "";
    [Required]
    [MinLength(4)]
    public required string Password { get; set; } = "";
    
    [Required] public string Gender { get; set; } = "";

    [Required] public string City { get; set; } = "";

    [Required] public string Country { get; set; } = "";

    [Required] public DateOnly DateOfBirth { get; set; }

}
