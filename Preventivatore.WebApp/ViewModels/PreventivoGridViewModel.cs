using System.Collections.Generic;

namespace Preventivatore.WebApp.ViewModels
{
    public class PreventivoGridViewModel
    {
        // i nomi delle colonne (es. "1000", "1001", ...)
        public List<string> CoverageColumns { get; set; } = new();

        // righe dei massimali
        public List<RowViewModel> CoverageRows { get; set; } = new();

        // righe delle franchigie
        public List<RowViewModel> DeductibleRows { get; set; } = new();
    }

    public class RowViewModel
    {
        // etichetta della riga (es. "20.000 a 30.000")
        public string Label { get; set; }

        // valori per ciascuna colonna
        public List<string> Values { get; set; } = new();
    }
}
