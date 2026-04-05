// src/Modules/Blog/Modules.Blog.Domain/Articles/Events/ArticlePublishedDomainEvent.cs
namespace Modules.Blog.Domain.Articles.Events;

using Modules.Common.Domain.Events;

public sealed record ArticlePublishedDomainEvent(Guid ArticleId, string Title) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}