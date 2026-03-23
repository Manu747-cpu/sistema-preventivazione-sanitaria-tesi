using System.ComponentModel.DataAnnotations;

namespace Preventivatore.Core.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Lo UserName è obbligatorio.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Password è obbligatoria.")]
        public string Password { get; set; } = string.Empty;
    }
}
