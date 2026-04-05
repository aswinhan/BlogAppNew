using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Modules.Common.Application.Storage;

namespace Modules.Common.Infrastructure.Storage;

public sealed class AzureBlobFileService(BlobServiceClient blobServiceClient) : IFileService
{
    private const string ContainerName = "uploads";

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        
        var blobClient = containerClient.GetBlobClient(uniqueFileName);

        var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        var uploadOptions = new BlobUploadOptions { HttpHeaders = blobHttpHeaders };

        await blobClient.UploadAsync(fileStream, uploadOptions, ct);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return;
        }

        try
        {
            var uri = new Uri(fileUrl);
            var blobName = Path.GetFileName(uri.LocalPath);
            
            var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
        }
        catch (UriFormatException)
        {
            // Ignore invalid URLs
        }
    }
}
