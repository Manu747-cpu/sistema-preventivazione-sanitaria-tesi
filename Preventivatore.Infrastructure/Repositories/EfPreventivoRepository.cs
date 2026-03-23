using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;

namespace Preventivatore.Infrastructure.Repositories
{
    public class EfPreventivoRepository : IPreventivoRepository
    {
        private readonly AppDbContext _context;

        public EfPreventivoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Preventivo entity)
        {
            await _context.Preventivi.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Preventivo?> GetByIdAsync(int id)
        {
            return await _context.Preventivi
                                 .Include(p => p.ServiziSelezionati)
                                 .Include(p => p.Documenti)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Preventivo>> ListByUtenteAsync(int utenteId)
        {
            return await _context.Preventivi
                                 .Where(p => p.UtenteId == utenteId)
                                 .Include(p => p.Polizza)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        // Endpoints generici:
        public async Task<IEnumerable<Preventivo>> ListAsync()
        {
            return await _context.Preventivi
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task UpdateAsync(Preventivo entity)
        {
            _context.Preventivi.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Preventivi.FindAsync(id);
            if (entity != null)
            {
                _context.Preventivi.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
