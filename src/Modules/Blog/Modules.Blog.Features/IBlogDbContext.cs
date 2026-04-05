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
    DbSet<Modules.Blog.Domain.Bookmarks.Bookmark> Bookmarks { get; }
    DbSet<Modules.Blog.Domain.Comments.Comment> Comments { get; }
    DbSet<Modules.Blog.Domain.Tags.Tag> Tags { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}