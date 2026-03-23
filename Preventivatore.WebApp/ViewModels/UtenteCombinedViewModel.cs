using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Preventivatore.WebApp.ViewModels
{
    public class UtenteCombinedViewModel
    {
        public int? SelectedMacroId { get; set; }
        public int? SelectedSubId { get; set; }

        public IEnumerable<SelectListItem> MacroCategorie { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> SubCategorie { get; set; } = new List<SelectListItem>();

        public string NomeSubCategoria { get; set; } = string.Empty;
        public string? NomeMacroCategoria { get; set; }

        public List<string> Colonne { get; set; } = new();
        public List<string> Righe { get; set; } = new();
        public List<List<string>> Celle { get; set; } = new();

        public List<DocumentoClienteViewModel> DocumentiDisponibili { get; set; } = new();
    }

    public class DocumentoClienteViewModel
    {
        public int Id { get; set; }
        public string NomeFile { get; set; } = string.Empty;
        public string NomePolizza { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}