using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preventivatore.Infrastructure.Data;

namespace Preventivatore.WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ServiziAggiuntiviController : Controller
    {
        private readonly AppDbContext _ctx;
        public ServiziAggiuntiviController(AppDbContext ctx) => _ctx = ctx;

        public IActionResult Index()
        {
            // TODO: lista ricarichi
            return View();
        }
    }
}
