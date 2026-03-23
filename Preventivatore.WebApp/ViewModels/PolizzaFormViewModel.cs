using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Preventivatore.WebApp.ViewModels
{
    public class PolizzaFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [StringLength(500)]
        public string? Descrizione { get; set; }

        [Required]
        [Display(Name = "Macrocategoria")]
        public int MacroCategoriaId { get; set; }

        public string? MacroCategoriaNome { get; set; }

        public IEnumerable<SelectListItem> MacroCategorie { get; set; } = new List<SelectListItem>();

        [Display(Name = "Importo Admin")]
        [Range(0, double.MaxValue)]
        public decimal ImportoAdmin { get; set; }

        [Display(Name = "% Admin")]
        [Range(0, 100)]
        public decimal PercentualeAdmin { get; set; }

        [Display(Name = "Servizi Aggiuntivi")]
        public List<int> SelectedServizi { get; set; } = new();

        public IEnumerable<SelectListItem> ServiziAggiuntivi { get; set; } = new List<SelectListItem>();

        public List<string> ServiziAggiuntiviNomi { get; set; } = new();

        [Display(Name = "Documenti")]
        public List<IFormFile> Documenti { get; set; } = new();

        public List<DocumentoPolizzaItemViewModel> ExistingDocumenti { get; set; } = new();
    }

    public class DocumentoPolizzaItemViewModel
    {
        public int Id { get; set; }
        public string NomeFile { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}