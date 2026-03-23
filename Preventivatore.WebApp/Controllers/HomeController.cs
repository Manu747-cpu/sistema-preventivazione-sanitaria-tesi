// Controllers/HomeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Preventivatore.Infrastructure.Data.Models;

namespace Preventivatore.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            if (await _userManager.IsInRoleAsync(user, "Customer"))
                return RedirectToAction("Seleziona", "Utente");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return View("Dashboard");

            return Forbid();
        }
    }
}