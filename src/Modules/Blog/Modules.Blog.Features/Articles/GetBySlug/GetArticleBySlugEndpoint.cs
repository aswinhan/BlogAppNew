using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;
using System;

namespace Modules.Blog.Features.Articles.GetBySlug;

public sealed record ArticleResponse(Guid Id, string Title, string Content, DateTime CreatedAtUtc);

public sealed record GetArticleBySlugQuery(string Slug) : IRequest<Result<ArticleResponse>>;

public sealed class GetArticleBySlugEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/blog/articles/{slug}", async (string slug, IMediator mediator) =>
        {
            var query = new GetArticleBySlugQuery(slug);
            var result = await mediator.Send(query);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return Results.NotFound(result.Error);
        });
    }
}
