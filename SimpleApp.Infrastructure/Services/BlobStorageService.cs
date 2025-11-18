using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SimpleApp.Application.Interfaces;

namespace SimpleApp.Infrastructure.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration config)
    {
        var connectionString = config["AzureBlobStorage:ConnectionString"];
        var containerName = config["AzureBlobStorage:ContainerName"];
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(fileName));
        await blobClient.UploadAsync(imageStream, true);
        return blobClient.Uri.ToString();
    }
}
