namespace Modules.Blog.Features.Comments.GetByArticle;

using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

public sealed record CommentResponse(
    Guid Id,
    string Content,
    string AuthorName,
    string? AuthorAvatarUrl,
    DateTime CreatedAtUtc,
    Guid? ParentCommentId,
    List<CommentResponse> Replies);

public sealed record GetArticleCommentsQuery(Guid ArticleId) : IRequest<Result<List<CommentResponse>>>;

public sealed class GetArticleCommentsEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/blog/articles/{articleId:guid}/comments", async (Guid articleId, IMediator mediator) =>
        {
            var query = new GetArticleCommentsQuery(articleId);
            var result = await mediator.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .WithTags("Blog");
    }
}
