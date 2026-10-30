using System;
using API.Entities;
using API.Helpers;

namespace API.Middleware;

public interface IMemberRepository
{
    void Update(Member member);
    Task<bool> SaveAllAsync();
    Task<PaginatedResult<Member>> GetMembersAsync(PagingParams pagingParams); // convention to suffix methods that return task with Async
    Task<Member?> GetMemberByIdAsync(string id);
    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
    Task<Member?> GetMemberForUpdatesAsync(string id); // different to GetMemberAsync because we need to get the User navagation property too that requires update when member is update 
}
