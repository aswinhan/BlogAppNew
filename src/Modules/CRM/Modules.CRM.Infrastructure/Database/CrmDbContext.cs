// src/Modules/CRM/Modules.CRM.Infrastructure/Database/CrmDbContext.cs
namespace Modules.CRM.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Modules.CRM.Domain.Subscribers;
using Modules.CRM.Features;

public sealed class CrmDbContext(DbContextOptions<CrmDbContext> options)
    : DbContext(options), ICrmDbContext
{
    public DbSet<Subscriber> Subscribers => Set<Subscriber>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CRITICAL: Database Isolation Rule
        modelBuilder.HasDefaultSchema("crm");

        modelBuilder.Entity<Subscriber>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.HasIndex(s => s.Email).IsUnique();
            builder.Property(s => s.Email).HasMaxLength(255).IsRequired();
            builder.Property(s => s.FirstName).HasMaxLength(100);
        });
    }
}