using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Preventivatore.Core.Entities;

namespace Preventivatore.Core.Interfaces
{
    public interface IPolizzaRepository
    {
        Task AddAsync(Polizza entity);
        Task<IEnumerable<Polizza>> GetAllAsync();
        Task<Polizza?> GetByIdAsync(int id);
        Task UpdateAsync(Polizza entity);
        Task DeleteAsync(int id);

    }
}
