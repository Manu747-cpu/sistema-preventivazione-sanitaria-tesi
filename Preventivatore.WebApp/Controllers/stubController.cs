using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Preventivatore.WebApp.Controllers
{
    [Authorize]
    public class StubController : Controller
    {
        [HttpGet]
        public IActionResult InCostruzione(string pagina)
        {
            ViewBag.Pagina = pagina;
            return View();
        }
    }
}
