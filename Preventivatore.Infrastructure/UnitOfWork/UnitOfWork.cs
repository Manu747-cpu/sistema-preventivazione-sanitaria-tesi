// Preventivatore.Infrastructure/UnitOfWork/UnitOfWork.cs
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;
using System.Threading.Tasks;

namespace Preventivatore.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ISubCategoriaRepository SubCategoriaRepository { get; }

        public UnitOfWork(
            AppDbContext context,
            ISubCategoriaRepository subCategoriaRepository)
        {
            _context = context;
            SubCategoriaRepository = subCategoriaRepository;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
