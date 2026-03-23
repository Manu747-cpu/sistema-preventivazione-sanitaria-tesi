using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Preventivatore.Core.Entities
{
    public class MacroCategoriaPolizza
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = null!;

        [MaxLength(500)]
        public string? Descrizione { get; set; }

        [MaxLength(200)]
        [DataType(DataType.ImageUrl)]
        public string? UrlImmagine { get; set; }

        public ICollection<Polizza>? Polizze { get; set; }


        public ICollection<SubCategoria>? SubCategorie { get; set; }
    }
}
