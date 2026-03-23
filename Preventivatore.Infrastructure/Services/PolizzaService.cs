using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using AutoMapper;
using Preventivatore.Core.DTOs;


namespace Preventivatore.Infrastructure.Services
{
    public class PolizzaService : IPolizzaService
    {
        private readonly IPolizzaRepository _repo;
        private readonly IMapper _mapper;

        public PolizzaService(IPolizzaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreatePolizzaDto dto)
        {
            var entity = _mapper.Map<Polizza>(dto);
            await _repo.AddAsync(entity);
            return entity.Id;
        }

        public async Task<IEnumerable<PolizzaDto>> ListAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<PolizzaDto>>(list);
        }

        public async Task<PolizzaDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<PolizzaDto>(e);
        }

        public async Task UpdateAsync(UpdatePolizzaDto dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null) return;
            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
        }

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}