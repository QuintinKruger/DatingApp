using System;
using API.Extensions;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary; // convention to prefix private field with '_'

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
    }
    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }

    public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            // file is not a dotnet resource thus we need to clean it up 
            // when we are done with it (dotnet has no other way to do it as it sits outside of CTR)
            // hence the `using`
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"), // force aspect ratio and crop to face using Cloudinary's features
                Folder = "da-ang22"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);


        }
        return uploadResult;
    }
}
