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

    public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
    {
        var query = context.Members.AsQueryable(); // create a deferred execution query
        
        query = query.Where(x => x.Id != memberParams.CurrentMemberId); // all members apart from current member

        if (memberParams.Gender != null)
        {
            // filter by gender
            query = query.Where(x => x.Gender == memberParams.Gender);
        }

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1)); // -1 because may not have had the birthday this year
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        // switch expression 
        query = memberParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive) // '_' represents default case
        };

        return await PaginationHelper.CreateAsync(query, memberParams.PageNumber, memberParams.PageSize);
        // return await context.Members.ToListAsync(); not pagination
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await context.Members
            .Where(x => x.Id == memberId)
            .SelectMany(x => x.Photos).ToListAsync();
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
