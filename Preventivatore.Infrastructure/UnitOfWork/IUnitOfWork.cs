using Preventivatore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preventivatore.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        ISubCategoriaRepository SubCategoriaRepository { get; }
        Task CompleteAsync();
    }
}
