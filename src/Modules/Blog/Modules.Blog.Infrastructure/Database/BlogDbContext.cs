namespace Modules.Blog.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Modules.Blog.Domain.Articles;
using Modules.Blog.Domain.Categories;
using Modules.Blog.Features;

public sealed class BlogDbContext(DbContextOptions<BlogDbContext> options)
    : DbContext(options), IBlogDbContext
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Modules.Blog.Domain.Authors.Author> Authors => Set<Modules.Blog.Domain.Authors.Author>();
    public DbSet<Modules.Blog.Domain.Bookmarks.Bookmark> Bookmarks => Set<Modules.Blog.Domain.Bookmarks.Bookmark>();
    public DbSet<Modules.Blog.Domain.Comments.Comment> Comments => Set<Modules.Blog.Domain.Comments.Comment>();
    public DbSet<Modules.Blog.Domain.Tags.Tag> Tags => Set<Modules.Blog.Domain.Tags.Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CRITICAL: Database Isolation Rule. Identity knows nothing of this schema.
        modelBuilder.HasDefaultSchema("blog");

        modelBuilder.Entity<Article>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => a.Slug).IsUnique();
            builder.Property(a => a.Title).HasMaxLength(200).IsRequired();
            builder.Property(a => a.Slug).HasMaxLength(250).IsRequired();

            // Category Relationship
            builder.HasOne<Modules.Blog.Domain.Categories.Category>()
                .WithMany()
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.Slug).IsUnique();
            builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Slug).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<Modules.Blog.Domain.Authors.Author>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Email).HasMaxLength(255).IsRequired();
            builder.Property(a => a.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Modules.Blog.Domain.Bookmarks.Bookmark>(builder =>
        {
            builder.HasKey(b => new { b.UserId, b.ArticleId });
            builder.HasOne<Modules.Blog.Domain.Articles.Article>()
                .WithMany()
                .HasForeignKey(b => b.ArticleId);
        });

        modelBuilder.Entity<Modules.Blog.Domain.Comments.Comment>(builder =>
        {
            builder.Property(c => c.Content).HasMaxLength(1000).IsRequired();

            builder.HasOne<Modules.Blog.Domain.Articles.Article>()
                .WithMany()
                .HasForeignKey(c => c.ArticleId);

            builder.HasOne<Modules.Blog.Domain.Comments.Comment>()
                .WithMany()
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Modules.Blog.Domain.Tags.Tag>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.Slug).IsUnique();
            builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
            builder.Property(t => t.Slug).HasMaxLength(50).IsRequired();
            // EF Core will automatically create the Many-to-Many relationship based on navigation properties
        });

        modelBuilder.ApplyConfiguration(new Modules.Common.Infrastructure.Outbox.OutboxMessageConfiguration());
    }
}
