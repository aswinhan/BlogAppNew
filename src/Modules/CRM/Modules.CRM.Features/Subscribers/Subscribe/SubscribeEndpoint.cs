// src/Modules/CRM/Modules.CRM.Features/Subscribers/Subscribe/SubscribeEndpoint.cs
namespace Modules.CRM.Features.Subscribers.Subscribe;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

public sealed record SubscribeRequest(string Email, string FirstName);
public sealed record SubscribeCommand(string Email, string FirstName) : IRequest<Result>;

public sealed class SubscribeEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/crm/subscribers", async (SubscribeRequest request, IMediator mediator) =>
        {
            var command = new SubscribeCommand(request.Email, request.FirstName);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
        })
        .WithTags("CRM");
    }
}