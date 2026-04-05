// src/Modules/Identity/Modules.Identity.Features/Users/Login/LoginUserEndpoint.cs
namespace Modules.Identity.Features.Users.Login;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

public sealed record LoginUserRequest(string Email, string Password);
public sealed record LoginUserCommand(string Email, string Password) : IRequest<Result<string>>;

public sealed class LoginUserEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/identity/login", async (LoginUserRequest request, IMediator mediator) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok(new { Token = result.Value }) : Results.BadRequest(result.Error);
        })
        .WithTags("Identity")
        .AllowAnonymous(); // Explicitly open to the public
    }
}