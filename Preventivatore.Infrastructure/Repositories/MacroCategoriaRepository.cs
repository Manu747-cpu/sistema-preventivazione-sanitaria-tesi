using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;

namespace Preventivatore.Infrastructure.Repositories
{
    public class MacroCategoriaRepository : IMacroCategoriaRepository
    {
        private readonly AppDbContext _ctx;
        public MacroCategoriaRepository(AppDbContext ctx)
            => _ctx = ctx;

        public async Task AddAsync(MacroCategoriaPolizza cat)
            => await _ctx.MacroCategorie.AddAsync(cat);

        public async Task DeleteAsync(int id)
        {
            var e = await _ctx.MacroCategorie.FindAsync(id);
            if (e != null)
                _ctx.MacroCategorie.Remove(e);
        }

        public async Task<MacroCategoriaPolizza?> GetByIdAsync(int id)
            => await _ctx.MacroCategorie.FindAsync(id);

        public async Task<IEnumerable<MacroCategoriaPolizza>> GetAllAsync()
            => await _ctx.MacroCategorie
                         .AsNoTracking()
                         .ToListAsync();

        public Task UpdateAsync(MacroCategoriaPolizza cat)
        {
            _ctx.MacroCategorie.Update(cat);
            return Task.CompletedTask;
        }
    }
}
