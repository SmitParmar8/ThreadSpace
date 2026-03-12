using Microsoft.EntityFrameworkCore;
using ThreadSpace.API.Models;

namespace ThreadSpace.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<AiPost> AiPosts => Set<AiPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name).IsUnique();

        modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.PostId, v.UserId }).IsUnique();
    }
}
