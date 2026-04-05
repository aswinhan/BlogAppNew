namespace Modules.Blog.Features.Articles.Search;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;
using Modules.Blog.Features.Articles.GetFeed;

public sealed record SearchArticlesQuery(string SearchTerm, int Page, int PageSize) : IRequest<Result<PagedResponse<ArticleFeedResponse>>>;

public sealed class SearchArticlesEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/blog/articles/search", async (string? q, IMediator mediator, int page = 1, int pageSize = 10) =>
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Results.BadRequest("Search term is required.");
            }

            var query = new SearchArticlesQuery(q, page, pageSize);
            var result = await mediator.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .AllowAnonymous()
        .WithTags("Blog");
    }
}
