using System.ComponentModel.DataAnnotations;

namespace Preventivatore.Core.Entities
{
    public class DocumentoPolizza
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string NomeFile { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = null!;

        public int PolizzaId { get; set; }
        public Polizza Polizza { get; set; } = null!;
    }
}