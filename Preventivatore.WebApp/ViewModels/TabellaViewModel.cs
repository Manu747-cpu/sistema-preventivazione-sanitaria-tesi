// TabellaViewModel.cs
using System.Collections.Generic;

namespace Preventivatore.WebApp.ViewModels
{
    public class TabellaViewModel
    {
        public string NomeSubCategoria { get; set; } = "";
        public List<string> Colonne { get; set; } = new();
        public List<string> Righe { get; set; } = new();
        public List<List<string>> Celle { get; set; } = new();
    }
}
