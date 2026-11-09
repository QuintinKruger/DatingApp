using System;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IPhotoRepository
{
    Task<PaginatedResult<Photo>> GetUnapprovedPhotos(int pageNumber, int pageSize);

    Task<Photo?> GetPhotoById(int id);

    void RemovePhoto(Photo photo);

    void Update(Photo photo);
}
