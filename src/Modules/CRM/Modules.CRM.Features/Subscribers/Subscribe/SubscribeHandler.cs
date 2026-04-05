// src/Modules/CRM/Modules.CRM.Features/Subscribers/Subscribe/SubscribeHandler.cs
namespace Modules.CRM.Features.Subscribers.Subscribe;

using MediatR;
using Modules.Common.Domain.Results;
using Modules.Common.Domain.Errors;
using Modules.CRM.Domain.Subscribers;

public sealed class SubscribeHandler(ICrmDbContext dbContext) : IRequestHandler<SubscribeCommand, Result>
{
    public async Task<Result> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        if (dbContext.Subscribers.Any(s => s.Email == request.Email))
        {
            return Result.Failure(new Error("Crm.AlreadySubscribed", "This email is already subscribed."));
        }

        var subscriber = Subscriber.Create(request.Email, request.FirstName);

        dbContext.Subscribers.Add(subscriber);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}