using System;

namespace Preventivatore.WebApp.ViewModels
{
    public class AdminPreventivoViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int SubCategoriaId { get; set; }
        public string SubCategoriaNome { get; set; } = string.Empty;

        public string RowKey { get; set; } = string.Empty;
        public string ColKey { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public DateTime DataCreazione { get; set; }
        public string Stato { get; set; } = string.Empty;
    }
}