using Preventivatore.Core.DTOs;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Preventivatore.Infrastructure.Services
{
    public class SubCategoriaService : ISubCategoriaService
    {
        private readonly ISubCategoriaRepository _repo;
        public SubCategoriaService(ISubCategoriaRepository repo) => _repo = repo;

        public async Task<List<SubCategoriaDto>> ListByMacroIdAsync(int macroId)
        {
            var e = await _repo.GetByMacroAsync(macroId);
            return e.Select(x => new SubCategoriaDto
            {
                Id = x.Id,
                Nome = x.Nome,
                MacroCategoriaPolizzaId = x.MacroCategoriaPolizzaId,
                Colonne = x.Colonne.OrderBy(c => c.Ordine).Select(c => c.Intestazione).ToList(),
                Righe = x.Righe.OrderBy(r => r.Ordine)
                          .Select(r => System.Text.Json.JsonSerializer.Deserialize<List<string>>(r.CelleJson)!)
                          .ToList()
            }).ToList();
        }

        public async Task<SubCategoriaDto?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            return new SubCategoriaDto
            {
                Id = x.Id,
                Nome = x.Nome,
                MacroCategoriaPolizzaId = x.MacroCategoriaPolizzaId,
                Colonne = x.Colonne.OrderBy(c => c.Ordine).Select(c => c.Intestazione).ToList(),
                Righe = x.Righe.OrderBy(r => r.Ordine)
                            .Select(r => System.Text.Json.JsonSerializer.Deserialize<List<string>>(r.CelleJson)!)
                            .ToList()
            };
        }

        public async Task<int> CreateAsync(SubCategoriaDto dto)
        {
            var ent = new SubCategoria
            {
                Nome = dto.Nome,
                MacroCategoriaPolizzaId = dto.MacroCategoriaPolizzaId,
                Colonne = dto.Colonne.Select((h, i) => new SubCategoriaColonna { Intestazione = h, Ordine = i }).ToList(),
                Righe = dto.Righe.Select((row, i) => new SubCategoriaRiga { Ordine = i, CelleJson = System.Text.Json.JsonSerializer.Serialize(row) }).ToList()
            };
            _repo.Add(ent);
            return ent.Id;
        }

        public async Task UpdateAsync(int id, SubCategoriaDto dto)
        {
            var ent = await _repo.GetByIdAsync(id);
            ent.Nome = dto.Nome;
            ent.Colonne = dto.Colonne.Select((h, i) => new SubCategoriaColonna { Intestazione = h, Ordine = i }).ToList();
            ent.Righe = dto.Righe.Select((row, i) => new SubCategoriaRiga { Ordine = i, CelleJson = System.Text.Json.JsonSerializer.Serialize(row) }).ToList();
            _repo.Update(ent);
        }

        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
