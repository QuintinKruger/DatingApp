using System;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRespository(AppDbContext context) : ILikesRepository
{
    public void AddLike(MemberLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(MemberLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
    {
        // projection in the world of entity framework is a Select - so we are making use of projection here
        return await context.Likes
            .Where(x => x.SourceMemberId == memberId)
            .Select(x => x.TargetMemberId)
            .ToListAsync();
    }

    public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
    {
        return await context.Likes
            .FindAsync(sourceMemberId, targetMemberId);
    }


    public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams)
    {
        var query = context.Likes.AsQueryable();
        IQueryable<Member> result;


        switch (likesParams.Predicate)
        {
            // users where the user with memberId has liked
            case "liked":
                result = query
                    .Where(x => x.SourceMemberId == likesParams.MemberId)
                    .Select(x => x.TargetMember);
                break;
            // users where the user with memberId has was liked by 
            case "likedBy":
                result = query.Where(x => x.TargetMemberId == likesParams.MemberId)
                    .Select(x => x.SourceMember);
                break;
            // where member liked a member and that member liked current member        
            default:
                var likeIds = await GetCurrentMemberLikeIds(likesParams.MemberId);
                result = query
                    .Where(x => x.TargetMemberId == likesParams.MemberId
                        && likeIds.Contains(x.SourceMemberId))
                    .Select(x => x.SourceMember);
                break;
        }

        return await PaginationHelper.CreateAsync(result, likesParams.PageNumber, likesParams.PageSize);
    }
}
