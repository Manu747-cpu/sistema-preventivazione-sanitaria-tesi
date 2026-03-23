using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Preventivatore.Core.Entities;
using System.Collections.Generic;


namespace Preventivatore.Core.Entities
{
    public enum TipoServizio
    {
        Fisso,
        Percentuale
    }

    public class ServizioAggiuntivo
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; } = null!;

        [Required]
        public TipoServizio TipoImporto { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Valore { get; set; }

        // FK verso Polizza
        public int PolizzaId { get; set; }
        public Polizza Polizza { get; set; } = null!;
    }
}
