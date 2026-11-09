using System;

namespace API.DTOs;

public class PhotoForApprovalDto
{
    public int PhotoId { get; set; }
    public required string Url { get; set; }
    public required string Username { get; set; }
    public bool IsApproved { get; set; }

}
