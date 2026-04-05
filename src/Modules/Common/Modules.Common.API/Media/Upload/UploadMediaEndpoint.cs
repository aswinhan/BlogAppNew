using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Media.Upload;

namespace Modules.Common.API.Media.Upload;

public sealed class UploadMediaEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/media", async (IFormFile file, IMediator mediator) =>
        {
            if (file is null || file.Length == 0)
            {
                return Results.BadRequest("File is required.");
            }

            using var stream = file.OpenReadStream();
            var command = new UploadMediaCommand(stream, file.FileName, file.ContentType);
            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                return Results.Ok(new { Url = result.Value });
            }

            return Results.BadRequest(result.Error);
        })
        .WithTags("Media")
        .DisableAntiforgery()
        .RequireAuthorization();
    }
}
