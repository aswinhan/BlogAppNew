// src/Modules/Blog/Modules.Blog.Features/Articles/Create/CreateArticleEndpoint.cs
namespace Modules.Blog.Features.Articles.Create;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

public sealed record CreateArticleRequest(string Title, string Content, Guid AuthorId, Guid CategoryId);
public sealed record CreateArticleCommand(string Title, string Content, Guid AuthorId, Guid CategoryId) : IRequest<Result<Guid>>;

public sealed class CreateArticleEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/blog/articles", async (CreateArticleRequest request, IMediator mediator) =>
        {
            var command = new CreateArticleCommand(request.Title, request.Content, request.AuthorId, request.CategoryId);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .WithTags("Blog")
        .RequireAuthorization();
    }
}