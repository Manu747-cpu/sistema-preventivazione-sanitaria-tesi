using System;
using System.Collections.Generic;

namespace Preventivatore.WebApp.ViewModels
{
    public class PreventivoDettaglioViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        public int SubCategoriaId { get; set; }
        public string SubCategoriaNome { get; set; } = string.Empty;

        public string RowKey { get; set; } = string.Empty;
        public string ColKey { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public DateTime DataCreazione { get; set; }
        public string Stato { get; set; } = string.Empty;

        public string? MacroCategoriaNome { get; set; }

        public List<PreventivoDocumentoViewModel> Documenti { get; set; } = new();
    }

    public class PreventivoDocumentoViewModel
    {
        public int Id { get; set; }
        public string NomeFile { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? NomePolizza { get; set; }
    }
}