// src/Modules/Identity/Modules.Identity.Infrastructure/Database/IdentityDbContext.cs
namespace Modules.Identity.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Modules.Identity.Domain.Users;
using Modules.Identity.Features.Users;
using System.Reflection.Emit;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : DbContext(options), IIdentityDbContext
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CRITICAL: Database Isolation Rule
        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(u => u.LastName).HasMaxLength(100);
        });
    }
}