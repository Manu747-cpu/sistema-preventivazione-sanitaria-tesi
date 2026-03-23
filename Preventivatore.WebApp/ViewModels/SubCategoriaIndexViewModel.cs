namespace Preventivatore.WebApp.ViewModels
{
    public class SubCategoriaIndexViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public List<string> Colonne { get; set; } = new();
        public List<string> Righe { get; set; } = new();
        public List<List<string>> Celle { get; set; } = new();
    }
}
