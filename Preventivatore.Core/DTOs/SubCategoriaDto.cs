using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preventivatore.Core.DTOs
{
    public class SubCategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public int MacroCategoriaPolizzaId { get; set; }
        public List<string> Colonne { get; set; } = new();
        public List<List<string>> Righe { get; set; } = new();
    }
}

