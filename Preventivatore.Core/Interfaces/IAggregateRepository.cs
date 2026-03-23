using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Preventivatore.Core.Interfaces
{

    public interface IAggregateRepository<T> where T : class
    {

        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(T aggregate, CancellationToken cancellationToken = default);

        Task UpdateAsync(T aggregate, CancellationToken cancellationToken = default);


        Task DeleteAsync(T aggregate, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default);
    }
}
