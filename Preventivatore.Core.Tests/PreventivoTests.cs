using System;
using System.Collections.Generic;

namespace Preventivatore.Core.Aggregates
{
    public class Preventivo
    {
        public Guid Id { get; private set; }
        public string Cliente { get; private set; } = string.Empty;
        public decimal ImportoLordo { get; private set; }
        public decimal ImportoNetto { get; private set; }
        public string Stato { get; private set; }
        public DateTime DataCreazione { get; private set; }

        private readonly List<decimal> _variazioni;
        public IReadOnlyCollection<decimal> Variazioni => _variazioni.AsReadOnly();

        /// <summary>
        /// Costruttore principale. Calcola automaticamente l'importo netto
        /// (aliquota fissa 22%). Stato iniziale: "Nuovo". DataCreazione = UTC now.
        /// </summary>
        public Preventivo(Guid id, string cliente, decimal importoLordo)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id non valido", nameof(id));
            if (string.IsNullOrWhiteSpace(cliente)) throw new ArgumentException("Cliente non valido", nameof(cliente));
            if (importoLordo < 0) throw new ArgumentException("ImportoLordo non può essere negativo", nameof(importoLordo));

            Id = id;
            Cliente = cliente;
            ImportoLordo = importoLordo;
            ImportoNetto = CalculateNetto(importoLordo);
            Stato = "Nuovo";
            DataCreazione = DateTime.UtcNow;
            _variazioni = new List<decimal>();
        }

        private decimal CalculateNetto(decimal lordo)
        {
            const decimal aliquota = 0.22m;
            // Arrotondo a 2 decimali
            return decimal.Round(lordo * (1 - aliquota), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Applica una variazione (positiva o negativa) all'importo lordo:
        /// - aggiorna ImportoLordo
        /// - ricalcola ImportoNetto
        /// - registra la variazione
        /// </summary>
        public void ApplicaVariazione(decimal delta)
        {
            ImportoLordo += delta;
            ImportoNetto = CalculateNetto(ImportoLordo);
            _variazioni.Add(delta);
        }

        /// <summary>
        /// Permette di cambiare lo stato del preventivo (es. "Approvato", "Rifiutato", ecc.).
        /// </summary>
        public void CambiaStato(string nuovoStato)
        {
            if (string.IsNullOrWhiteSpace(nuovoStato)) throw new ArgumentException("Stato non valido", nameof(nuovoStato));
            Stato = nuovoStato;
        }
    }
}
