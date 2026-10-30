using System;

namespace API.Extensions;

// strongly typed configurations from appsettings.json
public class CloudinarySettings
{
    public required string CloudName { get; set; }

    public required string ApiKey { get; set; }

    public required string ApiSecret { get; set; }
}
