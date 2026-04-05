using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Common.Application.Storage;
using Modules.Common.Domain.Results;

namespace Modules.Common.Application.Media.Upload;

public record UploadMediaCommand(Stream FileStream, string FileName, string ContentType) : IRequest<Result<string>>;

internal sealed class UploadMediaHandler(IFileService fileService) : IRequestHandler<UploadMediaCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        var url = await fileService.UploadAsync(request.FileStream, request.FileName, request.ContentType, cancellationToken);
        return Result.Success(url);
    }
}
