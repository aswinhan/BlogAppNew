// src/DigitalPlatform.Api/BackgroundJobs/BlogOutboxProcessor.cs
namespace DigitalPlatform.Api.BackgroundJobs;

using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Blog.Features;
using Modules.Blog.Infrastructure.Database;
using Modules.Common.Domain.Events;
using Modules.Common.Infrastructure.Outbox;

public sealed class BlogOutboxProcessor(
    IServiceProvider serviceProvider,
    ILogger<BlogOutboxProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Poll every 10 seconds (In production, use a library like Quartz.NET or Hangfire)
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            // 1. Fetch unprocessed messages
            var messages = await dbContext.Set<OutboxMessage>()
                .Where(m => m.ProcessedOnUtc == null)
                .Take(20)
                .ToListAsync(stoppingToken);

            if (messages.Count == 0) continue;

            // 2. Process and broadcast
            foreach (var message in messages)
            {
                try
                {
                    // Dynamically resolve the event type from the Blog Domain Assembly
                    var eventType = typeof(Modules.Blog.Domain.Articles.Events.ArticlePublishedDomainEvent).Assembly
                        .GetType($"Modules.Blog.Domain.Articles.Events.{message.Type}");

                    if (eventType is not null)
                    {
                        var domainEvent = JsonSerializer.Deserialize(message.Content, eventType) as IDomainEvent;
                        if (domainEvent is not null)
                        {
                            // Publish to MediatR (CRM will catch this!)
                            await mediator.Publish(domainEvent, stoppingToken);
                        }
                    }

                    message.ProcessedOnUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    message.Error = ex.Message;
                    logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                }
            }

            // 3. Save state
            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}