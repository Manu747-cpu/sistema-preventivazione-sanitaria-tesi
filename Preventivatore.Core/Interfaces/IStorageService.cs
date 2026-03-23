using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Preventivatore.Core.Interfaces
{
    public interface IStorageService
    {
        Task<string> SaveFileAsync(Stream stream, string folder, string fileName);

        Task<Uri> UploadAsync(Stream stream, string blobName);

        Task<Stream?> DownloadAsync(string blobName);

        Task<IEnumerable<string>> ListAsync();

        Task DeleteAsync(string blobName);
    }
}
