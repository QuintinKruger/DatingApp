using System;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository(AppDbContext context) : IPhotoRepository
{
    public async Task<Photo?> GetPhotoById(int id)
    {
        return await context.Photos.FindAsync(id);
    }

    public Task<PaginatedResult<Photo>> GetUnapprovedPhotos(int pageNumber, int pageSize)
    {
        var query = context.Photos.Where(x => x.IsApproved == null);
        return PaginationHelper.CreateAsync(query, pageNumber, pageSize);
    }

    public void RemovePhoto(Photo photo)
    {
       context.Photos.Remove(photo);
    }

    public void Update(Photo photo)
    {
        context.Entry(photo).State = EntityState.Modified;
    }
}
