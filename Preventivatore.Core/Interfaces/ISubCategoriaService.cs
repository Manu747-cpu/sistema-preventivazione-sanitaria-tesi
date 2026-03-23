using Preventivatore.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Preventivatore.Core.Interfaces
{
    public interface ISubCategoriaService
    {
        Task<List<SubCategoriaDto>> ListByMacroIdAsync(int macroCategoriaId);
        Task<SubCategoriaDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(SubCategoriaDto dto);
        Task UpdateAsync(int id, SubCategoriaDto dto);
        Task DeleteAsync(int id);
    }
}
