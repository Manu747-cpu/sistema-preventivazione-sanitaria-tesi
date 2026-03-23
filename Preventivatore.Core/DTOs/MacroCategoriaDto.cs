using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Preventivatore.Core.Entities;    // MacroCategoriaPolizza
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;



namespace Preventivatore.Core.DTOs
{
    public class MacroCategoriaDto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = null!;

        public string? Descrizione { get; set; }

        public string? UrlImmagine { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }

        public List<SubCategoriaDto> SubCategorie { get; set; } = new();

        public MacroCategoriaPolizza ToEntity() => new()
        {
            Id = Id,
            Nome = Nome,
            Descrizione = Descrizione,
            UrlImmagine = UrlImmagine
        };
    }
}
