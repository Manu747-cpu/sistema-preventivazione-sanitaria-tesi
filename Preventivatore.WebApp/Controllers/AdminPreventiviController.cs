using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Infrastructure.Data;
using Preventivatore.Infrastructure.Data.Models;
using Preventivatore.WebApp.ViewModels;

namespace Preventivatore.WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminPreventiviController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminPreventiviController(
            AppDbContext ctx,
            UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? stato)
        {
            var query = _ctx.PreventiviMvp.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p =>
                    p.SubCategoriaNome.Contains(q) ||
                    p.RowKey.Contains(q) ||
                    p.ColKey.Contains(q) ||
                    p.Value.Contains(q) ||
                    p.UserId.Contains(q)
                );
            }

            if (!string.IsNullOrWhiteSpace(stato))
            {
                stato = stato.Trim();
                query = query.Where(p => p.Stato == stato);
            }

            var items = await query
                .OrderByDescending(p => p.DataCreazione)
                .ToListAsync();

            var userIds = items
                .Select(x => x.UserId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var utenti = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email
                })
                .ToListAsync();

            var utentiDict = utenti.ToDictionary(
                x => x.Id,
                x => new { x.UserName, x.Email });

            var lista = items.Select(p =>
            {
                utentiDict.TryGetValue(p.UserId, out var u);

                return new AdminPreventivoViewModel
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserName = u?.UserName ?? "(utente non trovato)",
                    Email = u?.Email ?? "",
                    SubCategoriaId = p.SubCategoriaId,
                    SubCategoriaNome = p.SubCategoriaNome,
                    RowKey = p.RowKey,
                    ColKey = p.ColKey,
                    Value = p.Value,
                    DataCreazione = p.DataCreazione,
                    Stato = p.Stato
                };
            }).ToList();

            ViewBag.Q = q;
            ViewBag.Stato = stato;

            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Dettagli(int id)
        {
            var item = await _ctx.PreventiviMvp
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound();

            var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == item.UserId);

            var vm = new AdminPreventivoViewModel
            {
                Id = item.Id,
                UserId = item.UserId,
                UserName = user?.UserName ?? "(utente non trovato)",
                Email = user?.Email ?? "",
                SubCategoriaId = item.SubCategoriaId,
                SubCategoriaNome = item.SubCategoriaNome,
                RowKey = item.RowKey,
                ColKey = item.ColKey,
                Value = item.Value,
                DataCreazione = item.DataCreazione,
                Stato = item.Stato
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiaStato(int id, string stato)
        {
            var allowed = new[] { "Bozza", "Confermato", "Rifiutato" };
            if (!allowed.Contains(stato))
                return BadRequest("Stato non valido.");

            var item = await _ctx.PreventiviMvp.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
                return NotFound();

            item.Stato = stato;
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Elimina(int id)
        {
            var item = await _ctx.PreventiviMvp.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
                return NotFound();

            _ctx.PreventiviMvp.Remove(item);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}