using System;

namespace Modules.Blog.Domain.Bookmarks;

public sealed class Bookmark
{
    public Guid UserId { get; private set; }
    public Guid ArticleId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Bookmark()
    {
    }

    public static Bookmark Create(Guid userId, Guid articleId)
    {
        return new Bookmark
        {
            UserId = userId,
            ArticleId = articleId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
