using System;

namespace API.DTOs;

public class LoginDto
{
    public string Email { get; set; } = ""; // adding empty string to prevent complaints from IDE but doing this because we will enforce validation at login level for this DTO
    public string Password { get; set; }

}
