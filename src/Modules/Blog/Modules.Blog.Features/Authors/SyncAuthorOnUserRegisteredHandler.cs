namespace Modules.Blog.Features.Authors;

using MediatR;
using Modules.Blog.Domain.Authors;
using Modules.Identity.Domain.Users.Events;
using System.Threading;
using System.Threading.Tasks;

public sealed class SyncAuthorOnUserRegisteredHandler(IBlogDbContext dbContext) : INotificationHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        var author = Author.Create(notification.UserId, notification.Email, notification.FirstName);

        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}