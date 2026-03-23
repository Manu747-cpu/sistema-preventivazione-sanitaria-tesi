using System.ComponentModel.DataAnnotations;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;



namespace Preventivatore.Core.DTOs
{
    public class UpdatePreventivoDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "L'importo lordo deve essere un valore positivo.")]
        public decimal ImportoLordo { get; set; }

        //decremento dal prezzo finale
        public decimal Delta { get; set; }

        [Required]
        [StringLength(50)]
        public string Stato { get; set; } = string.Empty;
    }
}
