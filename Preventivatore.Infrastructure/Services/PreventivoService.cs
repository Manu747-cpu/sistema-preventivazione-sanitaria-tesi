using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;


namespace Preventivatore.Infrastructure.Services
{
    public class PreventivoService : IPreventivoService
    {
        private readonly IPreventivoRepository _repo;
        private readonly IMapper _mapper;

        public PreventivoService(IPreventivoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreatePreventivoDto dto)
        {
            var entity = _mapper.Map<Preventivo>(dto);
            await _repo.AddAsync(entity);
            return entity.Id;
        }

        public async Task<IEnumerable<PreventivoDto>> GetAllAsync()
        {
            var list = await _repo.ListAsync();
            return _mapper.Map<IEnumerable<PreventivoDto>>(list);
        }

        public async Task<PreventivoDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<PreventivoDto>(e);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePreventivoDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return false;
            _mapper.Map(dto, e);
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
