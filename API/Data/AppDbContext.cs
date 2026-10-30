using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

// abstraction of the DB
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    
    // tell DbContext about of entities 
    public DbSet<AppUser> Users {get; set;} // represents Users table in DB
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
}
