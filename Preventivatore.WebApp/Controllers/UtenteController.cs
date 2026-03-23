using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Infrastructure.Data;
using Preventivatore.WebApp.ViewModels;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Preventivatore.WebApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class UtenteController : Controller
    {
        private readonly AppDbContext _ctx;

        public UtenteController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // Landing iniziale
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Seleziona));
        }

        // GET: schermata selezione
        [HttpGet]
        public async Task<IActionResult> Seleziona()
        {
            var vm = new UtenteCombinedViewModel
            {
                MacroCategorie = await _ctx.MacroCategorie
                    .AsNoTracking()
                    .OrderBy(m => m.Nome)
                    .Select(m => new SelectListItem(m.Nome, m.Id.ToString()))
                    .ToListAsync(),
                SubCategorie = new List<SelectListItem>()
            };

            return View(vm);
        }

        // POST: mostra tabella + documenti
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Seleziona(UtenteCombinedViewModel vm)
        {
            vm.MacroCategorie = await _ctx.MacroCategorie
                .AsNoTracking()
                .OrderBy(m => m.Nome)
                .Select(m => new SelectListItem(m.Nome, m.Id.ToString()))
                .ToListAsync();

            if (vm.SelectedMacroId.HasValue)
            {
                vm.SubCategorie = await _ctx.SubCategorie
                    .AsNoTracking()
                    .Where(s => s.MacroCategoriaPolizzaId == vm.SelectedMacroId.Value)
                    .OrderBy(s => s.Nome)
                    .Select(s => new SelectListItem(s.Nome, s.Id.ToString()))
                    .ToListAsync();

                vm.NomeMacroCategoria = await _ctx.MacroCategorie
                    .AsNoTracking()
                    .Where(m => m.Id == vm.SelectedMacroId.Value)
                    .Select(m => m.Nome)
                    .FirstOrDefaultAsync();
            }
            else
            {
                vm.SubCategorie = new List<SelectListItem>();
            }

            if (vm.SelectedSubId.HasValue)
            {
                var entity = await _ctx.SubCategorie
                    .Include(s => s.Colonne)
                    .Include(s => s.Righe)
                    .Include(s => s.MacroCategoriaPolizza)
                    .FirstOrDefaultAsync(s => s.Id == vm.SelectedSubId.Value);

                if (entity != null)
                {
                    vm.NomeSubCategoria = entity.Nome;
                    vm.NomeMacroCategoria = entity.MacroCategoriaPolizza?.Nome;

                    vm.Colonne = entity.Colonne
                        .OrderBy(c => c.Ordine)
                        .Select(c => c.Intestazione)
                        .ToList();

                    vm.Righe = entity.Righe
                        .OrderBy(r => r.Ordine)
                        .Select(r => r.Label)
                        .ToList();

                    vm.Celle = entity.Righe
                        .OrderBy(r => r.Ordine)
                        .Select(r => JsonSerializer.Deserialize<List<string>>(r.CelleJson) ?? new List<string>())
                        .ToList();

                    var macroId = entity.MacroCategoriaPolizzaId;

                    vm.DocumentiDisponibili = await _ctx.DocumentiPolizza
                        .AsNoTracking()
                        .Where(d => d.Polizza.MacroCategoriaId == macroId)
                        .OrderBy(d => d.Polizza.Nome)
                        .ThenBy(d => d.NomeFile)
                        .Select(d => new DocumentoClienteViewModel
                        {
                            Id = d.Id,
                            NomeFile = d.NomeFile,
                            NomePolizza = d.Polizza.Nome,
                            Url = d.Url
                        })
                        .ToListAsync();
                }
            }

            return View(vm);
        }

        // AJAX: sottocategorie per macrocategoria
        [HttpGet]
        public async Task<IActionResult> GetSubCategorie(int macroId)
        {
            var items = await _ctx.SubCategorie
                .AsNoTracking()
                .Where(s => s.MacroCategoriaPolizzaId == macroId)
                .OrderBy(s => s.Nome)
                .Select(s => new
                {
                    id = s.Id,
                    nome = s.Nome
                })
                .ToListAsync();

            return Json(items);
        }
    }
}