// File: ViewModels/MacroCategoriaViewModel.cs
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Preventivatore.WebApp.ViewModels
{
    public class MacroCategoriaViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = "";

        public string? Descrizione { get; set; }

        public string? UrlImmagine { get; set; }

        [Display(Name = "Image")]
        public IFormFile? Image { get; set; }

        public List<SubCategoriaViewModel> SubCategorie { get; set; } = new();
    }
}
