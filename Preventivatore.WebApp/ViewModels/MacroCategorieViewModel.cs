// MacroCategorieViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Preventivatore.WebApp.ViewModels
{
    public class MacroCategorieViewModel
    {
        [Required(ErrorMessage = "Devi selezionare una macrocategoria")]
        [Display(Name = "Macrocategoria")]
        public string SelectedId { get; set; } = "";

        public List<SelectListItem> MacroCategorie { get; set; }
            = new List<SelectListItem>();
    }
}
