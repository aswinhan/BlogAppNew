namespace Modules.Blog.Features.Comments.Add;

using System;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

public sealed record AddCommentRequest(string Content, Guid? ParentCommentId);

public sealed record AddCommentCommand(Guid ArticleId, Guid UserId, string Content, Guid? ParentCommentId) : IRequest<Result<Guid>>;

public sealed class AddCommentEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/blog/articles/{articleId:guid}/comments", async (Guid articleId, AddCommentRequest request, HttpContext context, IMediator mediator) =>
        {
            var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new AddCommentCommand(articleId, userId, request.Content, request.ParentCommentId);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .WithTags("Blog")
        .RequireAuthorization();
    }
}
