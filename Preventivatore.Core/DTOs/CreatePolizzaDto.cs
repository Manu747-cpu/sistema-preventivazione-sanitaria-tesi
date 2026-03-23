// CreatePolizzaDto.cs
using System.ComponentModel.DataAnnotations;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;



namespace Preventivatore.Core.DTOs
{
    public class CreatePolizzaDto
    {
        [Required]
        public string Nome { get; set; } = null!;
        public string? Descrizione { get; set; }
        public decimal PremioBase { get; set; }

        [Required]
        public int MacroCategoriaId { get; set; }
        // aggiungi qui gli altri campi necessari
    }

    public class UpdatePolizzaDto : CreatePolizzaDto
    {
        [Required]
        public int Id { get; set; }
    }
}
