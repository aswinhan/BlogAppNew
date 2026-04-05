namespace Modules.Blog.Features.Bookmarks.GetSaved;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;
using Modules.Blog.Features.Articles.GetFeed; // Imports the DTOs from the other feature

public sealed record GetSavedArticlesQuery(Guid UserId, int Page, int PageSize) : IRequest<Result<PagedResponse<ArticleFeedResponse>>>;

public sealed class GetSavedArticlesEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/blog/bookmarks", async (ClaimsPrincipal user, IMediator mediator, int page = 1, int pageSize = 10) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Results.Unauthorized();
            }

            var query = new GetSavedArticlesQuery(userId, page, pageSize);
            var result = await mediator.Send(query);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return Results.BadRequest(result.Error);
        })
        .WithTags("Blog")
        .RequireAuthorization();
    }
}