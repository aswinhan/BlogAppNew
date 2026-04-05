using System;
using Modules.Common.Domain.Primitives;

namespace Modules.Blog.Domain.Comments;

public sealed class Comment : Entity
{
    public Guid ArticleId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; }
    public Guid? ParentCommentId { get; private set; }

    private Comment()
    {
        ArticleId = default;
        UserId = default;
        Content = default!;
        ParentCommentId = default;
    }

    public static Comment Create(Guid articleId, Guid userId, string content, Guid? parentCommentId = null)
    {
        return new Comment
        {
            Id = Guid.NewGuid(),
            ArticleId = articleId,
            UserId = userId,
            Content = content,
            ParentCommentId = parentCommentId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
