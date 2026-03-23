using System.ComponentModel.DataAnnotations;

namespace Preventivatore.Core.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Lo UserName è obbligatorio.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L’Email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Email non valida.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Password è obbligatoria.")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "La Password deve essere almeno di 6 caratteri.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il Role è obbligatorio.")]
        public string Role { get; set; } = string.Empty;
    }
}
