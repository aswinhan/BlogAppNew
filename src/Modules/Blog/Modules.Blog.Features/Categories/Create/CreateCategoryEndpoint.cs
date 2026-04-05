using System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

namespace Modules.Blog.Features.Categories.Create;

public sealed record CreateCategoryRequest(string Name);

public sealed record CreateCategoryCommand(string Name) : IRequest<Result<Guid>>;

public sealed class CreateCategoryEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/blog/categories", async (CreateCategoryRequest request, IMediator mediator) =>
        {
            var command = new CreateCategoryCommand(request.Name);
            var result = await mediator.Send(command);

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
