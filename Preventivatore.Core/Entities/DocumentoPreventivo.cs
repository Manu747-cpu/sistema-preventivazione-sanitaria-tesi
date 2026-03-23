using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preventivatore.Core.Entities
{
    public class DocumentoPreventivo
    {
        public int Id { get; set; }
        public int PreventivoId { get; set; }
        public string NomeFile { get; set; } = null!;
        public string Url { get; set; } = null!;
        public DateTime CaricatoIl { get; set; } = DateTime.UtcNow;
        public Preventivo Preventivo { get; set; } = null!;
    }
}

