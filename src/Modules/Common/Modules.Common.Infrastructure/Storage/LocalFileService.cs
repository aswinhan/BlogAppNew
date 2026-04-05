using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Modules.Common.Application.Storage;

namespace Modules.Common.Infrastructure.Storage;

public sealed class LocalFileService(IWebHostEnvironment environment) : IFileService
{
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var uploadPath = Path.Combine(environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, uniqueFileName);

        using var fileStreamToWrite = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(fileStreamToWrite, ct);

        return $"/uploads/{uniqueFileName}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.CompletedTask;
        }

        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
