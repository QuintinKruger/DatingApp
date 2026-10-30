using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

// request deletegate is availble to any middleware component for consumption
// logger is the style used to grab a hold of the logger for this class 
// IHostEnvironment is used to tell what env we are in
public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment env)
// : IMiddleware // not strictly needed to implement the interface 
// Microsoft is trying to do away with boiler plate code 
// but to me this is the point of defining an interface 
// Microsoft being microsoft once again - deciding weird things 
// I am however leaving it here to be explicit 
{

    // this method name is expected to exist in ANY middleware component
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); // pass on to next piece of middleware
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{message}", ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = env.IsDevelopment()
            ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace)
            : new ApiException(context.Response.StatusCode, ex.Message, "Internal server error");

            // return response as proper json as expected by http standards 
            // middleware does not adhere to the standard by default 
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);

        }
    }
}
