// file: Preventivatore.Infrastructure/Services/MacroCategoriaService.cs
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.UnitOfWork;

namespace Preventivatore.Infrastructure.Services
{
    public class MacroCategoriaService : IMacroCategoriaService
    {
        private readonly IMacroCategoriaRepository _repo;
        private readonly IStorageService _storage;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MacroCategoriaService(
            IMacroCategoriaRepository repo,
            IStorageService storage,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _repo = repo;
            _storage = storage;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(
            MacroCategoriaPolizza entity,
            Stream? imageStream,
            string? fileName)
        {
            if (imageStream is not null && fileName is not null)
            {
                entity.UrlImmagine = await _storage
                    .SaveFileAsync(imageStream, "macrocategorie", fileName);
            }

            await _repo.AddAsync(entity);
            await _uow.CompleteAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(
            MacroCategoriaPolizza entity,
            Stream? imageStream,
            string? fileName)
        {
            if (imageStream is not null && fileName is not null)
            {
                entity.UrlImmagine = await _storage
                    .SaveFileAsync(imageStream, "macrocategorie", fileName);
            }

            await _repo.UpdateAsync(entity);
            await _uow.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<MacroCategoriaDto>> ListAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<MacroCategoriaDto>>(entities);
        }

        public async Task<MacroCategoriaDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e is null ? null : _mapper.Map<MacroCategoriaDto>(e);
        }
    }
}
