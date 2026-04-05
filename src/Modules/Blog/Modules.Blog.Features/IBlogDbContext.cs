// src/Modules/Blog/Modules.Blog.Features/IBlogDbContext.cs
namespace Modules.Blog.Features;

using Microsoft.EntityFrameworkCore;
using Modules.Blog.Domain.Articles;
using Modules.Blog.Domain.Categories;

public interface IBlogDbContext
{
    DbSet<Article> Articles { get; }
    DbSet<Category> Categories { get; }
    DbSet<Modules.Blog.Domain.Authors.Author> Authors { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}