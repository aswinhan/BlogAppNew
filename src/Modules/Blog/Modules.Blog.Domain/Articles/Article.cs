// src/Modules/Blog/Modules.Blog.Domain/Articles/Article.cs
namespace Modules.Blog.Domain.Articles;

using Modules.Common.Domain.Primitives;
using Modules.Blog.Domain.Articles.Events;

public sealed class Article : Entity
{
    public string Title { get; private set; }
    public string Slug { get; private set; }
    public string Content { get; private set; }
    public Guid AuthorId { get; private set; }
    public bool IsPublished { get; private set; }
    public Guid CategoryId { get; private set; }

    private Article()
    {
        Title = default!;
        Slug = default!;
        Content = default!;
    }

    public static Article Create(string title, string content, Guid authorId, Guid categoryId)
    {
        // Basic slug generation (in a real app, use a robust Regex/Slugifier)
        var slug = title.ToLower().Replace(" ", "-").Replace(PunctuationToStrip(), "");

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Content = content,
            AuthorId = authorId,
            IsPublished = false,
            CategoryId = categoryId
        };

        return article;
    }

    public void Publish()
    {
        if (IsPublished) return;

        IsPublished = true;
        ModifiedAtUtc = DateTime.UtcNow;

        // This event will trigger the Outbox pattern later
        RaiseDomainEvent(new ArticlePublishedDomainEvent(Id, Title));
    }

    private static string PunctuationToStrip() => ".,?;:'\"!@#$%^&*()_+={}[]|\\<>/~`";
}