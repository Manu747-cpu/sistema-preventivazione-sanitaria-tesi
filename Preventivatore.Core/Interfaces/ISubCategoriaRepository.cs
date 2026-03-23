using Preventivatore.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Preventivatore.Core.Interfaces
{
    public interface ISubCategoriaRepository
    {
        Task<IEnumerable<SubCategoria>> GetByMacroAsync(int macroCategoriaId);
        Task<SubCategoria> GetByIdAsync(int id);
        void Add(SubCategoria entity);
        void Update(SubCategoria entity);
        Task DeleteAsync(int id);
    }
}
