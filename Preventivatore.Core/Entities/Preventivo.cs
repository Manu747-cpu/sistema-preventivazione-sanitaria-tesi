using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Preventivatore.Core.Entities
{
    public class Preventivo
    {
        [Key]
        public int Id { get; set; }

        // FK su utente (se lo gestisci)
        public int UtenteId { get; set; }
        [ForeignKey(nameof(UtenteId))]

        // Polizza scelta
        public int PolizzaId { get; set; }
        [ForeignKey(nameof(PolizzaId))]
        public Polizza Polizza { get; set; } = null!;

        // Data di creazione
        public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

        // Somma di premio + ricarichi + servizi
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotaleFinale { get; set; }

        // Percentuale o importo del ricarico
        [Column(TypeName = "decimal(18,2)")]
        public decimal RicaricoApplicato { get; set; }

        // Relazioni 1:N
        public ICollection<PreventivoServizioAggiuntivo> ServiziSelezionati { get; set; }
            = new List<PreventivoServizioAggiuntivo>();

        public ICollection<DocumentoPreventivo> Documenti { get; set; }
            = new List<DocumentoPreventivo>();
    }
}
