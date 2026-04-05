// src/Modules/CRM/Modules.CRM.Features/ICrmDbContext.cs
namespace Modules.CRM.Features;

using Microsoft.EntityFrameworkCore;
using Modules.CRM.Domain.Subscribers;

public interface ICrmDbContext
{
    DbSet<Subscriber> Subscribers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}