using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Preventivatore.Core/Entities/SubCategoria.cs
namespace Preventivatore.Core.Entities
{
    public class SubCategoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public int MacroCategoriaPolizzaId { get; set; }
        public MacroCategoriaPolizza MacroCategoriaPolizza { get; set; } = null!;

        public ICollection<SubCategoriaColonna> Colonne { get; set; } = new List<SubCategoriaColonna>();
        public ICollection<SubCategoriaRiga> Righe { get; set; } = new List<SubCategoriaRiga>();
    }

    public class SubCategoriaColonna
    {
        public int Id { get; set; }
        public int SubCategoriaId { get; set; }
        public SubCategoria SubCategoria { get; set; } = null!;
        public string Intestazione { get; set; } = "";
        public int Ordine { get; set; }
    }

    public class SubCategoriaRiga
    {
        public int Id { get; set; }
        public int SubCategoriaId { get; set; }
        public SubCategoria SubCategoria { get; set; } = null!;
        public int Ordine { get; set; }
        public string CelleJson { get; set; } = "";
        public string Label { get; set; }
    }
}
