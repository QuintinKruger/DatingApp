using System;
using API.Entities;
using API.Helpers;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await context.Members.FindAsync(id);
    }

    public async Task<Member?> GetMemberForUpdatesAsync(string id)
    {
        return await context.Members
            .Include(x => x.User)
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PaginatedResult<Member>> GetMembersAsync(PagingParams pagingParams)
    {
        var query = context.Members.AsQueryable(); // create a deferred execution query
        return await PaginationHelper.CreateAsync(query, pagingParams.PageNumber, pagingParams.PageSize);
        // return await context.Members.ToListAsync(); not pagination
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await context.Members
            .Where(x => x.Id == memberId)
            .SelectMany(x => x.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    // not strictly needed - change tracking is already setup for us 
    // for the Member entity by EF. 
    public void Update(Member member)
    {
        // with change detection by EF we manually set the Modified state to true 
        // for EF to perform a save 
        context.Entry(member).State = EntityState.Modified;
    }
}
