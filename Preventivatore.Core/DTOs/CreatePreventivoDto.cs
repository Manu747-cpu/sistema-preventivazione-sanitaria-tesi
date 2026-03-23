using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;



namespace Preventivatore.Core.DTOs
{
    public class CreatePreventivoDto
    {
        [Required]
        public int UtenteId { get; set; }

        [Required]
        public int PolizzaId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Devi selezionare almeno un servizio.")]
        public List<int> ServiziIds { get; set; } = new();

        [Range(0, double.MaxValue, ErrorMessage = "Il ricarico deve essere positivo o zero.")]
        public decimal Ricarico { get; set; }
    }
}
