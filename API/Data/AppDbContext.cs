using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

// abstraction of the DB
public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    
    // tell DbContext about of entities 
    // public DbSet<AppUser> Users {get; set;} // represents Users table in DB
    // above already provided by IdentityDbContext
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }

    public DbSet<Message> Messages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // model builder can be used to override entity framework functionality 
        // here we are overriding the inability for sql-lite to store times as UTC format (dates with a Z suffix)
        base.OnModelCreating(modelBuilder); 

        // different way of seeding data into our DB
        modelBuilder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole{Id = "member-id", ConcurrencyStamp="a", Name="Member", NormalizedName="MEMBER"},
                new IdentityRole{Id = "moderator-id", ConcurrencyStamp="b", Name="Moderator", NormalizedName="MODERATOR"},
                new IdentityRole{Id = "admin-id", ConcurrencyStamp="c", Name="Admin", NormalizedName="ADMIN"}
            );


        // no conventiion for many-to-many relationship on a self referenced 
        // table, thus resort to configuration ourselves. We are creating a join table
        // recall this means defining a 2 many-to-one relationships with a join table
        // Member }-| MemberLike (join table) |-{ Member 
        modelBuilder.Entity<MemberLike>()
            .HasKey(x => new {x.SourceMemberId, x.TargetMemberId}); // composite primary key for MemberLike table
        modelBuilder.Entity<MemberLike>()
            .HasOne(s => s.SourceMember)
            .WithMany(t => t.LikedMembers)
            .HasForeignKey(s => s.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<MemberLike>()
            .HasOne(s => s.TargetMember)
            .WithMany(t => t.LikedByMembers)
            .HasForeignKey(s => s.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>()
            .HasOne(x => x.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict); // dont delete messages received when message deleted
        modelBuilder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict); // dont delete messages received when message deleted


        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v =>  v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
        );

        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {   
                if(property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
                
            }
        }
    }
}
