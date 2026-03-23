using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Services; // per IStorageService


namespace Preventivatore.Infrastructure.Services
{
    public interface IPreventivoService
    {
        Task<int> CreateAsync(CreatePreventivoDto dto);
        Task<IEnumerable<PreventivoDto>> GetAllAsync();
        Task<PreventivoDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdatePreventivoDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
