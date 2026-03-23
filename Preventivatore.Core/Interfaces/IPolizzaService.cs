using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Entities;

namespace Preventivatore.Core.Interfaces
{
    public interface IPolizzaService
    {
        Task<int> CreateAsync(CreatePolizzaDto dto);
        Task<IEnumerable<PolizzaDto>> ListAsync();
        Task<PolizzaDto?> GetByIdAsync(int id);
        Task UpdateAsync(UpdatePolizzaDto dto);
        Task DeleteAsync(int id);

    }
}
