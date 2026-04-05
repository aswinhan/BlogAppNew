using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;
using System;
using System.Collections.Generic;

namespace Modules.Blog.Features.Articles.GetFeed;

public record ArticleFeedResponse(Guid Id, string Title, string Slug, string CategoryName, string AuthorName, DateTime CreatedAtUtc);

public record PagedResponse<T>(List<T> Items, int TotalCount, int Page, int PageSize);

public record GetArticlesQuery(int Page = 1, int PageSize = 10) : IRequest<Result<PagedResponse<ArticleFeedResponse>>>;

public class GetArticlesEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/blog/articles", async (ISender sender, int page = 1, int pageSize = 10) =>
        {
            var query = new GetArticlesQuery(page, pageSize);
            var result = await sender.Send(query);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return Results.BadRequest(result.Error);
        })
        .AllowAnonymous();
    }
}
