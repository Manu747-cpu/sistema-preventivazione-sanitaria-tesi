using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;

namespace Preventivatore.Infrastructure.Repositories
{
    public class EfMacroCategoriaRepository : IMacroCategoriaRepository
    {
        private readonly AppDbContext _ctx;
        public EfMacroCategoriaRepository(AppDbContext ctx)
            => _ctx = ctx;

        public async Task AddAsync(MacroCategoriaPolizza entity)
            => await _ctx.MacroCategorie.AddAsync(entity);

        public Task UpdateAsync(MacroCategoriaPolizza entity)
        {
            _ctx.MacroCategorie.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _ctx.MacroCategorie.FindAsync(id);
            if (e != null)
                _ctx.MacroCategorie.Remove(e);
        }

        public async Task<MacroCategoriaPolizza?> GetByIdAsync(int id)
            => await _ctx.MacroCategorie.FindAsync(id);

        public async Task<IEnumerable<MacroCategoriaPolizza>> GetAllAsync()
            => await _ctx.MacroCategorie.ToListAsync();
    }
}
