// file: Preventivatore.WebApp/Models/DashboardViewModel.cs
using System.Collections.Generic;
using Preventivatore.Core.DTOs;

namespace Preventivatore.WebApp.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<MacroCategoriaDto> Categories { get; set; }
    }
}
