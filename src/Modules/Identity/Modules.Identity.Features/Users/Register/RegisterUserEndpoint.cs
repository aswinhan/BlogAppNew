// src/Modules/Identity/Modules.Identity.Features/Users/Register/RegisterUserEndpoint.cs
namespace Modules.Identity.Features.Users.Register;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

public sealed record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);
public sealed record RegisterUserCommand(string Email, string Password, string FirstName, string LastName) : IRequest<Result>;

public sealed class RegisterUserEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/identity/register", async (RegisterUserRequest request, IMediator mediator) =>
        {
            var command = new RegisterUserCommand(request.Email, request.Password, request.FirstName, request.LastName);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
        })
        .WithTags("Identity");
    }
}