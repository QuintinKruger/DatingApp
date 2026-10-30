using System.Text;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    // specify the connection string name that contains connection info stored in our configuration 
    // file (appsettings.json or other environment files like appsettings.Development.json)
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); 
}); // tell app about our DBContext and provide it with options
builder.Services.AddCors(); // to add CORS header to responses for clients to accept cross origin content  
// builder.Services.AddTransient() // new instance of service everytime its queried from service container
builder.Services.AddScoped<ITokenService, TokenService>(); // lives for the duration of an HTTP request 
builder.Services.AddScoped<IPhotoService, PhotoService>();
// see https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/service-lifetimes fore more`

// add repository to be used in app
builder.Services.AddScoped<IMemberRepository, MemberRepository>(); 

// 'bind' configuration defined in appsettings.json, section CloudinarySettings to the CloudinarySettings class
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// define middleware for authenticating using JWT providing decryption strategy using symetric key 
// (similar to how we setup encryption in TokenService)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token key not found");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline. 
// for middleware, does something on request coming in before sending it through our app or before sending it out from our app
// ordering is NB!
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors( x => x.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

// add seed data to our DB
// service locator pattern used below - this is because we do not have access to our dependencies here
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync(); // create database using migrations if not already exists
    await Seed.SeedUsers(context);

} catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex,"An error occurred during migration");
    throw;
}

app.Run();
