using JwtAuthenticationDemo.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationDemo.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .HasIndex(x => x.Email)
        .IsUnique();
    }

    public DbSet<User> Users => Set<User>();
}