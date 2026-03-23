// Preventivatore.WebApp/ViewModels/SubCategoriaViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Preventivatore.WebApp.ViewModels
{
    public class SubCategoriaViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = "";


        public int MacroCategoriaPolizzaId { get; set; }

        // angolo e header
        public string CornerText { get; set; } = "Fatturato";
        public string TableHeader { get; set; } = "Massimali di copertura";

        // Intestazioni colonne
        public List<string> Colonne { get; set; } = new();


        // Etichette righe + celle
        public List<string> Righe { get; set; } = new();
        public List<List<string>> Celle { get; set; } = new();
    }
}
