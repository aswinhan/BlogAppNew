using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Modules.Common.Application.Storage;

public interface IFileService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);

    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
}
