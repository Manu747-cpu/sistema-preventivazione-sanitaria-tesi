namespace Preventivatore.WebApp.ViewModels
{
    public class PolizzaGridViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public decimal ImportoAdmin { get; set; }
        public decimal PercentualeAdmin { get; set; }
        public decimal ImportoModeratore { get; set; }
        public decimal PercentualeModeratore { get; set; }
    }
}
