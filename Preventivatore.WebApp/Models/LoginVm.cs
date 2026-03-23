using System.ComponentModel.DataAnnotations;

namespace Preventivatore.WebApp.Models
{
    public class LoginVm
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        // Per fare redirect dopo il login
        public string? ReturnUrl { get; set; }
    }
}
