// src/Modules/Identity/Modules.Identity.Features/Users/IIdentityDbContext.cs
namespace Modules.Identity.Features.Users;

using Microsoft.EntityFrameworkCore;
using Modules.Identity.Domain.Users;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}