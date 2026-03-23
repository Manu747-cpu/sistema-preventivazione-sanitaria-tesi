using System;
using System.Collections.Generic;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;


namespace Preventivatore.Core.DTOs
{
    public class PreventivoDto
    {
        public int Id { get; set; }

        public string Cliente { get; set; } = string.Empty;
        public decimal ImportoLordo { get; set; }
        public decimal ImportoNetto { get; set; }
        public string Stato { get; set; } = string.Empty;
        public DateTime DataCreazione { get; set; }
        public int PolizzaId { get; set; }
        public required List<int> ServiziIds { get; set; }
        public decimal Ricarico { get; set; }
        public decimal TotaleFinale { get; set; }

        public List<PreventivoFileDto> Files { get; set; } = new();
        public List<decimal> Variazioni { get; set; } = new();
    }
}
