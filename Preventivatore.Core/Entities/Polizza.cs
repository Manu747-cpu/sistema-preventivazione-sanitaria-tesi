using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Preventivatore.Core.Entities;


namespace Preventivatore.Core.Entities
{
    public class Polizza
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string? Descrizione { get; set; }
        public int MacroCategoriaId { get; set; }
        public MacroCategoriaPolizza MacroCategoria { get; set; } = null!;

        public ICollection<ServizioAggiuntivo> ServiziAggiuntivi { get; set; }
            = new List<ServizioAggiuntivo>();

        public ICollection<DocumentoPolizza> DocumentiPolizza { get; set; }
            = new List<DocumentoPolizza>();

        // ← inserisci questo:
        public ICollection<RicaricoUtente> RicarichiUtente { get; set; }
            = new List<RicaricoUtente>();

    }
}
