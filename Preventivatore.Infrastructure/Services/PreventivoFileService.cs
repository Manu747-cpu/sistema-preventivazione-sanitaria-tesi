using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;
using Preventivatore.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Preventivatore.Core.Entities;





namespace Preventivatore.Infrastructure.Services
{
    public class PreventivoFileService : IPreventivoFileService
    {
        private readonly AppDbContext _ctx;
        private readonly BlobServiceClient _blobClient;
        private readonly IMapper _mapper;

        public PreventivoFileService(
            AppDbContext ctx,
            BlobServiceClient blobClient,
            IMapper mapper)
        {
            _ctx = ctx;
            _blobClient = blobClient;
            _mapper = mapper;
        }

        public async Task<int> UploadAsync(Guid preventivoId, IFormFile file)
        {
            var container = _blobClient.GetBlobContainerClient("preventivi");
            await container.CreateIfNotExistsAsync();

            var unique = Guid.NewGuid().ToString("N");
            var blobName = $"{preventivoId}/{unique}_{file.FileName}";
            var blob = container.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, overwrite: false);

            var entity = new PreventivoFile
            {
                PreventivoId = preventivoId,
                BlobName = blobName,
                Url = blob.Uri.ToString(),
                UploadedAt = DateTime.UtcNow
            };

            _ctx.Set<PreventivoFile>().Add(entity);
            await _ctx.SaveChangesAsync();
            return entity.Id;
        }

        public Task<IEnumerable<PreventivoFileDto>> ListAsync(Guid preventivoId)
        {
            var files = _ctx.Set<PreventivoFile>()
                            .Where(f => f.PreventivoId == preventivoId)
                            .ToList();
            return Task.FromResult(_mapper.Map<IEnumerable<PreventivoFileDto>>(files));
        }

        public async Task<Stream?> DownloadAsync(Guid preventivoId, int fileId)
        {
            var meta = await _ctx.PreventivoFiles.FindAsync(fileId);
            if (meta == null || meta.PreventivoId != preventivoId)
                return null;

            var container = _blobClient.GetBlobContainerClient("preventivi");
            var blob = container.GetBlobClient(meta.BlobName);

            var ms = new MemoryStream();
            await blob.DownloadToAsync(ms);
            ms.Position = 0;
            return ms;
        }

        public async Task<bool> DeleteAsync(Guid preventivoId, int fileId)
        {
            var meta = await _ctx.PreventivoFiles.FindAsync(fileId);
            if (meta == null || meta.PreventivoId != preventivoId)
                return false;

            var container = _blobClient.GetBlobContainerClient("preventivi");
            await container.GetBlobClient(meta.BlobName).DeleteIfExistsAsync();

            _ctx.PreventivoFiles.Remove(meta);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
