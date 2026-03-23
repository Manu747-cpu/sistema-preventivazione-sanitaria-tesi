using System;
using System.ComponentModel.DataAnnotations;

namespace Preventivatore.Core.Entities
{
    public class PreventivoMvp
    {
        [Key]
        public int Id { get; set; }

        // Identity user id (string)
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public int SubCategoriaId { get; set; }

        [Required]
        public string RowKey { get; set; } = null!;

        [Required]
        public string SubCategoriaNome { get; set; } = null!;

        [Required]
        public string ColKey { get; set; } = null!;

        [Required]
        public string Value { get; set; } = null!;

        public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

        [Required]
        public string Stato { get; set; } = "Bozza";
    }
}