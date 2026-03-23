using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Services;

     public class AzureBlobStorageService : IStorageService
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorageService(
            BlobServiceClient blobServiceClient,
            IOptions<StorageSettings> settings)
        {
            // prendo il container già creato in appsettings.json (se non esiste lo creo)
            _containerClient = blobServiceClient
                .GetBlobContainerClient(settings.Value.ContainerName);
        }


private async Task EnsureContainerExistsAsync()
    {
        try
        {
            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        }
        catch
        {
            // ignora
        }
    }

    public async Task<string> SaveFileAsync(Stream stream, string folder, string fileName)
    {
        await EnsureContainerExistsAsync();
        var blobName = $"{folder}/{fileName}";
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(stream, overwrite: true);
        return blobClient.Uri.ToString();
    }

    public async Task<Uri> UploadAsync(Stream stream, string blobName)
    {
        await EnsureContainerExistsAsync();
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(stream, overwrite: true);
        return blobClient.Uri;
    }

    public async Task<Stream?> DownloadAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        try
        {
            var download = await blobClient.DownloadAsync();
            return download.Value.Content;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<IEnumerable<string>> ListAsync()
    {
        await EnsureContainerExistsAsync();
        var results = new List<string>();
            await foreach (BlobItem blob in _containerClient.GetBlobsAsync())
            results.Add(blob.Name);
        return results;
    }

        // ← implementa finalmente DeleteAsync
        // ← implementazione di DeleteAsync
        public async Task DeleteAsync(string blobName)
        {
            await EnsureContainerExistsAsync();
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
}
 
