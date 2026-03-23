// SubCategorieViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Preventivatore.WebApp.ViewModels
{
    public class SubCategorieViewModel
    {
        // arriverà dal form precedente
        public int MacroCategoriaId { get; set; }

        [Required(ErrorMessage = "Devi selezionare una sottocategoria")]
        [Display(Name = "Sottocategoria")]
        public string SelectedId { get; set; } = "";

        public List<SelectListItem> SubCategorie { get; set; }
            = new List<SelectListItem>();
    }
}
