using System.ComponentModel.DataAnnotations;
using Preventivatore.Core.Entities;

namespace Preventivatore.Core.DTOs
{
    public class PolizzaDto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = null!;

        public string? Descrizione { get; set; }

        [Required]
        public int MacroCategoriaId { get; set; }

        public Polizza ToEntity() => new()
        {
            Id = Id,
            Nome = Nome,
            Descrizione = Descrizione,
            MacroCategoriaId = MacroCategoriaId
        };
    }
}

