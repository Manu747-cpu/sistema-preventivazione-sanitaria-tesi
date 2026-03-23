using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;

namespace Preventivatore.Infrastructure.Repositories
{
    public class PolizzaRepository : IPolizzaRepository
    {
        private readonly AppDbContext _ctx;
        public PolizzaRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(Polizza entity)
            => await _ctx.Polizze.AddAsync(entity);

        public async Task<IEnumerable<Polizza>> GetAllAsync()
            => await _ctx.Polizze.ToListAsync();

        public async Task<Polizza?> GetByIdAsync(int id)
            => await _ctx.Polizze.FindAsync(id);

        public Task UpdateAsync(Polizza entity)
        {
            _ctx.Polizze.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var p = await GetByIdAsync(id);
            if (p != null) _ctx.Polizze.Remove(p);
        }
    }
}
