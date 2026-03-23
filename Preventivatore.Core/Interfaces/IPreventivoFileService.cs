using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Preventivatore.Core.DTOs;

namespace Preventivatore.Core.Interfaces
{
    public interface IPreventivoFileService
    {
       Task<int> UploadAsync(Guid preventivoId, IFormFile file);
       Task<IEnumerable<PreventivoFileDto>> ListAsync(Guid preventivoId);
       Task<Stream?> DownloadAsync(Guid preventivoId, int fileId);
       Task<bool> DeleteAsync(Guid preventivoId, int fileId);
    }
}
