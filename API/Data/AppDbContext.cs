using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

// abstraction of the DB
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    
    // tell DbContext about of entities 
    public DbSet<AppUser> Users {get; set;} // represents Users table in DB
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // model builder can be used to override entity framework functionality 
        // here we are overriding the inability for sql-lite to store times as UTC format (dates with a Z suffix)
        base.OnModelCreating(modelBuilder); 

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {   
                if(property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                
            }
        }
    }
}
