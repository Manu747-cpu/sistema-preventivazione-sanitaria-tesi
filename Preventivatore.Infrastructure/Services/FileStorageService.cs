using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Preventivatore.Core.Interfaces;

namespace Preventivatore.Infrastructure.Services
{
    public class FileStorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        // Local filesystem
        private readonly string _basePath;
        private readonly string _requestPath = "/uploads";

        // Azure / Azurite
        private readonly string? _storageConn;
        private readonly string _containerName;
        private readonly bool _blobConfigured;

        public FileStorageService(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;

            _basePath = Path.Combine(
                env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "uploads");

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            _storageConn =
                _config["Storage:ConnectionString"]
                ?? _config["StorageSettings:ConnectionString"]
                ?? _config.GetConnectionString("AzureBlobStorage");

            _containerName =
                _config["Storage:ContainerName"]
                ?? _config["StorageSettings:ContainerName"]
                ?? "preventivi";

            _blobConfigured = !string.IsNullOrWhiteSpace(_storageConn);
        }

        // ---------------------------
        // PUBLIC API
        // ---------------------------

        public async Task<string> SaveFileAsync(Stream stream, string folder, string fileName)
        {
            folder ??= string.Empty;
            var safeName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            // 1) prova Blob se configurato
            if (_blobConfigured)
            {
                try
                {
                    var container = await GetContainerAsync();

                    var blobName = string.IsNullOrWhiteSpace(folder)
                        ? safeName
                        : $"{NormalizeBlobName(folder)}/{safeName}";

                    var client = container.GetBlobClient(blobName);

                    stream.Position = 0;
                    await client.UploadAsync(stream, overwrite: true);

                    return client.Uri.ToString();
                }
                catch
                {
                    // fallback automatico su locale
                    if (stream.CanSeek)
                        stream.Position = 0;
                }
            }

            // 2) fallback locale
            return await SaveFileLocalAsync(stream, folder, safeName);
        }

        public async Task<Uri> UploadAsync(Stream stream, string blobName)
        {
            var folder = Path.GetDirectoryName(blobName)?.Replace("\\", "/") ?? "";
            var fileName = Path.GetFileName(blobName);

            var url = await SaveFileAsync(stream, folder, fileName);

            return Uri.TryCreate(url, UriKind.Absolute, out var abs)
                ? abs
                : new Uri(url, UriKind.Relative);
        }

        public async Task<Stream?> DownloadAsync(string blobName)
        {
            // 1) se è un URL assoluto Blob/Azurite, prova da Blob
            if (IsAbsoluteUrl(blobName) && _blobConfigured)
            {
                try
                {
                    var container = await GetContainerAsync();
                    var normalizedBlobName = ExtractBlobNameFromUrl(blobName, _containerName);

                    if (string.IsNullOrWhiteSpace(normalizedBlobName))
                        return null;

                    var client = container.GetBlobClient(normalizedBlobName);

                    if (!await client.ExistsAsync())
                        return null;

                    var ms = new MemoryStream();
                    await client.DownloadToAsync(ms);
                    ms.Position = 0;
                    return ms;
                }
                catch
                {
                    return null;
                }
            }

            // 2) se è path relativo locale
            var localRelative = NormalizeLocalRelativePath(blobName);
            var localFile = Path.Combine(_basePath, localRelative.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!File.Exists(localFile))
                return null;

            return File.OpenRead(localFile);
        }

        public async Task<IEnumerable<string>> ListAsync()
        {
            if (_blobConfigured)
            {
                try
                {
                    var container = await GetContainerAsync();
                    var results = new List<string>();

                    await foreach (var item in container.GetBlobsAsync())
                        results.Add(item.Name);

                    return results;
                }
                catch
                {
                    // fallback locale
                }
            }

            var files = Directory.GetFiles(_basePath, "*.*", SearchOption.AllDirectories);
            return files.Select(f =>
            {
                var rel = Path.GetRelativePath(_basePath, f);
                return rel.Replace(Path.DirectorySeparatorChar, '/');
            });
        }

        public async Task DeleteAsync(string blobName)
        {
            // 1) prova Blob se URL assoluto e blob configurato
            if (IsAbsoluteUrl(blobName) && _blobConfigured)
            {
                try
                {
                    var container = await GetContainerAsync();
                    var normalizedBlobName = ExtractBlobNameFromUrl(blobName, _containerName);

                    if (!string.IsNullOrWhiteSpace(normalizedBlobName))
                    {
                        var client = container.GetBlobClient(normalizedBlobName);
                        await client.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                        return;
                    }
                }
                catch
                {
                    // fallback locale
                }
            }

            // 2) elimina locale
            var localRelative = NormalizeLocalRelativePath(blobName);
            var localFile = Path.Combine(_basePath, localRelative.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(localFile))
                File.Delete(localFile);
        }

        // ---------------------------
        // HELPERS
        // ---------------------------

        private async Task<string> SaveFileLocalAsync(Stream stream, string folder, string safeName)
        {
            var normalizedFolder = NormalizeBlobName(folder);

            var targetDir = string.IsNullOrWhiteSpace(normalizedFolder)
                ? _basePath
                : Path.Combine(_basePath, normalizedFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            var filePath = Path.Combine(targetDir, safeName);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await stream.CopyToAsync(fs);
            }

            var url = string.IsNullOrWhiteSpace(normalizedFolder)
                ? $"{_requestPath}/{safeName}"
                : $"{_requestPath}/{normalizedFolder}/{safeName}";

            return url.Replace("\\", "/");
        }

        private async Task<BlobContainerClient> GetContainerAsync()
        {
            var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2023_11_03);
            var service = new BlobServiceClient(_storageConn!, options);
            var container = service.GetBlobContainerClient(_containerName);

            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            return container;
        }

        private static string NormalizeBlobName(string name)
        {
            return (name ?? string.Empty)
                .Replace("\\", "/")
                .Trim()
                .Trim('/');
        }

        private static bool IsAbsoluteUrl(string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out _);
        }

        private static string NormalizeLocalRelativePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var path = value.Replace("\\", "/").Trim();

            if (path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                path = path["/uploads/".Length..];
            else if (path.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                path = path["uploads/".Length..];
            else
                path = path.TrimStart('/');

            return path;
        }

        private static string ExtractBlobNameFromUrl(string absoluteUrl, string containerName)
        {
            if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out var uri))
                return string.Empty;

            var path = uri.AbsolutePath.Trim('/');

            var prefix = containerName.Trim('/') + "/";
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return path[prefix.Length..];

            return string.Empty;
        }
    }
}