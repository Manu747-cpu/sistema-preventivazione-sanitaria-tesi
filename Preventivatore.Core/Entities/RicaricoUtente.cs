using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preventivatore.Core.Entities
{
    public enum RuoloUtente
    {
        Admin,
        Moderatore
    }

    public class RicaricoUtente
    {
        public int Id { get; set; }

        public int PolizzaId { get; set; }
        public Polizza Polizza { get; set; } = null!;

        [Required, MaxLength(450)]
        public string UtenteId { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Percentuale { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Importo { get; set; }

        [Required]
        public RuoloUtente Ruolo { get; set; }
    }
}

