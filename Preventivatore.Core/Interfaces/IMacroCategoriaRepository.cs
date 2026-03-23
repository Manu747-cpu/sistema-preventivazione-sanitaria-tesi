using System.Collections.Generic;
using System.Threading.Tasks;
using Preventivatore.Core.Entities;

namespace Preventivatore.Core.Interfaces
{
    public interface IMacroCategoriaRepository
    {
        Task<IEnumerable<MacroCategoriaPolizza>> GetAllAsync();
        Task<MacroCategoriaPolizza?> GetByIdAsync(int id);
        Task AddAsync(MacroCategoriaPolizza entity);
        Task UpdateAsync(MacroCategoriaPolizza entity);
        Task DeleteAsync(int id);
    }
}
