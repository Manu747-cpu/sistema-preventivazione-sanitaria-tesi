using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Preventivatore.Infrastructure.Repositories
{
    public class SubCategoriaRepository : ISubCategoriaRepository
    {
        private readonly AppDbContext _ctx;
        public SubCategoriaRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<SubCategoria>> GetByMacroAsync(int macroCategoriaId)
            => await _ctx.SubCategorie
                         .Include(s => s.Colonne)
                         .Include(s => s.Righe)
                         .Where(s => s.MacroCategoriaPolizzaId == macroCategoriaId)
                         .AsNoTracking()
                         .ToListAsync();

        public async Task<SubCategoria> GetByIdAsync(int id)
            => await _ctx.SubCategorie
                         .Include(s => s.Colonne)
                         .Include(s => s.Righe)
                         .FirstOrDefaultAsync(s => s.Id == id)
               ?? throw new KeyNotFoundException($"SubCategoria {id} non trovata");

        public void Add(SubCategoria entity) => _ctx.SubCategorie.Add(entity);
        public void Update(SubCategoria entity) => _ctx.SubCategorie.Update(entity);
        public async Task DeleteAsync(int id)
        {
            var e = await GetByIdAsync(id);
            _ctx.SubCategorie.Remove(e);
        }
    }
}
