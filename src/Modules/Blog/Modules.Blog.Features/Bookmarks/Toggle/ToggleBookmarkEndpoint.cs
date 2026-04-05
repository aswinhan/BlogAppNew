namespace Modules.Blog.Features.Bookmarks.Toggle;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

// Note: We include both UserId and ArticleId in the command to keep the handler completely isolated from HTTP logic
public sealed record ToggleBookmarkCommand(Guid UserId, Guid ArticleId) : IRequest<Result>;

public sealed class ToggleBookmarkEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/blog/articles/{articleId:guid}/bookmarks/toggle", async (Guid articleId, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Results.Unauthorized();
            }

            var command = new ToggleBookmarkCommand(userId, articleId);
            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                return Results.Ok();
            }

            return Results.BadRequest(result.Error);
        })
        .WithTags("Blog")
        .RequireAuthorization();
    }
}