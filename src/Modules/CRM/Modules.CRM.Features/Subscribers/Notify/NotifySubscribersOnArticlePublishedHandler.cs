// src/Modules/CRM/Modules.CRM.Features/Subscribers/Notify/NotifySubscribersOnArticlePublishedHandler.cs
namespace Modules.CRM.Features.Subscribers.Notify;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Blog.Domain.Articles.Events;

// Notice this is an INotificationHandler, NOT an IRequestHandler
public sealed class NotifySubscribersOnArticlePublishedHandler(
    ICrmDbContext dbContext,
    ILogger<NotifySubscribersOnArticlePublishedHandler> logger)
    : INotificationHandler<ArticlePublishedDomainEvent>
{
    public async Task Handle(ArticlePublishedDomainEvent notification, CancellationToken cancellationToken)
    {
        // 1. Find all active subscribers
        var activeSubscribers = await dbContext.Subscribers
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);

        // 2. Send emails (Simulated for now)
        foreach (var subscriber in activeSubscribers)
        {
            logger.LogInformation(
                "CRM ALERT: Sending newsletter to {Email} for new article: '{ArticleTitle}'",
                subscriber.Email,
                notification.Title);

            // Here you would integrate MailKit, SendGrid, or AWS SES
        }
    }
}