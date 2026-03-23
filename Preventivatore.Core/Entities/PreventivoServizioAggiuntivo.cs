using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preventivatore.Core.Entities
{
    public class PreventivoServizioAggiuntivo
    {
        public int PreventivoId { get; set; }
        public int ServizioId { get; set; }                
        public decimal ImportoCalcolato { get; set; }    
        public Preventivo Preventivo { get; set; } = null!;
    }
}
