using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        // do something after execution of delegate (in this helpers case - after the request was handled in the controller)
        // '?' because we might not have the identity if the user is not making an authenticated 
        // request
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

        var memberId = resultContext.HttpContext.User.GetMemberId();

        // cannot dependency inject into this method thus use the service locator pattern to get db context
        var dbContext = resultContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        await dbContext.Members
            .Where(x => x.Id == memberId)
            .ExecuteUpdateAsync( // immediately performs an update rather than defer it to when SaveChanges called 
                setters => setters.SetProperty(x => x.LastActive, DateTime.UtcNow) // update the specified fields with the specified values
            ); 
    }
}
