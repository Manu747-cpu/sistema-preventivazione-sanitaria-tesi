using Preventivatore.Core.DTOs;
using Preventivatore.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Preventivatore.Core.Interfaces
{
    public interface IMacroCategoriaService
    {
        Task<int> CreateAsync(MacroCategoriaPolizza entity, Stream? image, string? fileName);
        Task<IEnumerable<MacroCategoriaDto>> ListAsync();
        Task<MacroCategoriaDto?> GetByIdAsync(int id);
        Task UpdateAsync(MacroCategoriaPolizza entity, Stream? image, string? fileName);
        Task DeleteAsync(int id);
    }
}
