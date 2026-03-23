using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Preventivatore.Core.Entities;

namespace Preventivatore.Core.Interfaces
{
    public interface IPreventivoRepository
    {
        Task AddAsync(Preventivo entity);
        Task<Preventivo?> GetByIdAsync(int id);
        Task<IEnumerable<Preventivo>> ListAsync();
        Task<IEnumerable<Preventivo>> ListByUtenteAsync(int utenteId);
        Task UpdateAsync(Preventivo entity);
        Task DeleteAsync(int id);
    }
}
