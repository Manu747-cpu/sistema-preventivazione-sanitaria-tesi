using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;
using Preventivatore.WebApp.ViewModels;
using System.Security.Claims;

namespace Preventivatore.WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    public class PolizzeController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IStorageService _storage;

        public PolizzeController(AppDbContext ctx, IStorageService storage)
        {
            _ctx = ctx;
            _storage = storage;
        }

        // GET: Polizze
        public async Task<IActionResult> Index()
        {
            var items = await _ctx.Polizze
                .Include(p => p.MacroCategoria)
                .Include(p => p.RicarichiUtente)
                .Include(p => p.DocumentiPolizza)
                .ToListAsync();

            var vm = items.Select(p => new PolizzaGridViewModel
            {
                Id = p.Id,
                Nome = p.Nome,
                Categoria = p.MacroCategoria.Nome,
                ImportoAdmin = p.RicarichiUtente
                    .FirstOrDefault(r => r.Ruolo == RuoloUtente.Admin)?.Importo ?? 0,
                PercentualeAdmin = p.RicarichiUtente
                    .FirstOrDefault(r => r.Ruolo == RuoloUtente.Admin)?.Percentuale ?? 0,
                ImportoModeratore = 0,
                PercentualeModeratore = 0
            }).ToList();

            return View(vm);
        }

        // GET: Polizze/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var p = await _ctx.Polizze
                .Include(x => x.MacroCategoria)
                .Include(x => x.RicarichiUtente)
                .Include(x => x.ServiziAggiuntivi)
                .Include(x => x.DocumentiPolizza)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return NotFound();

            var vm = new PolizzaFormViewModel
            {
                Id = p.Id,
                Nome = p.Nome,
                Descrizione = p.Descrizione,
                MacroCategoriaId = p.MacroCategoriaId,
                MacroCategoriaNome = p.MacroCategoria?.Nome,
                ImportoAdmin = p.RicarichiUtente
                    .FirstOrDefault(r => r.Ruolo == RuoloUtente.Admin)?.Importo ?? 0,
                PercentualeAdmin = p.RicarichiUtente
                    .FirstOrDefault(r => r.Ruolo == RuoloUtente.Admin)?.Percentuale ?? 0,
                SelectedServizi = p.ServiziAggiuntivi.Select(s => s.Id).ToList(),
                ServiziAggiuntiviNomi = p.ServiziAggiuntivi.Select(s => s.Nome).ToList(),
                ExistingDocumenti = p.DocumentiPolizza
                    .Select(d => new DocumentoPolizzaItemViewModel
                    {
                        Id = d.Id,
                        NomeFile = d.NomeFile,
                        Url = d.Url
                    })
                    .ToList()
            };

            return View(vm);
        }

        // GET: Polizze/Create
        public async Task<IActionResult> Create()
        {
            var vm = new PolizzaFormViewModel
            {
                MacroCategorie = await GetMacroCategorieAsync(),
                ServiziAggiuntivi = await GetServiziAsync()
            };

            return View(vm);
        }

        // POST: Polizze/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PolizzaFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.MacroCategorie = await GetMacroCategorieAsync();
                vm.ServiziAggiuntivi = await GetServiziAsync();
                return View(vm);
            }

            var polizza = new Polizza
            {
                Nome = vm.Nome,
                Descrizione = vm.Descrizione,
                MacroCategoriaId = vm.MacroCategoriaId
            };

            _ctx.Polizze.Add(polizza);
            await _ctx.SaveChangesAsync();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Challenge();

            var ricaricoAdmin = new RicaricoUtente
            {
                UtenteId = userId,
                PolizzaId = polizza.Id,
                Ruolo = RuoloUtente.Admin,
                Importo = vm.ImportoAdmin,
                Percentuale = vm.PercentualeAdmin
            };

            _ctx.RicarichiUtente.Add(ricaricoAdmin);

            foreach (var sid in vm.SelectedServizi)
            {
                var servizio = await _ctx.ServiziAggiuntivi.FindAsync(sid);
                if (servizio != null)
                    polizza.ServiziAggiuntivi.Add(servizio);
            }

            if (vm.Documenti != null && vm.Documenti.Any())
            {
                foreach (var file in vm.Documenti.Where(f => f != null && f.Length > 0))
                {
                    var uploaded = await _storage.UploadAsync(
                        file.OpenReadStream(),
                        $"polizze/{polizza.Id}/{file.FileName}");

                    var doc = new DocumentoPolizza
                    {
                        PolizzaId = polizza.Id,
                        NomeFile = file.FileName,
                        Url = uploaded.ToString()
                    };

                    _ctx.DocumentiPolizza.Add(doc);
                }
            }

            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Polizze/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _ctx.Polizze
                .Include(x => x.RicarichiUtente)
                .Include(x => x.ServiziAggiuntivi)
                .Include(x => x.DocumentiPolizza)
                .Include(x => x.MacroCategoria)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return NotFound();

            var vm = new PolizzaFormViewModel
            {
                Id = p.Id,
                Nome = p.Nome,
                Descrizione = p.Descrizione,
                MacroCategoriaId = p.MacroCategoriaId,
                MacroCategoriaNome = p.MacroCategoria?.Nome,
                ImportoAdmin = p.RicarichiUtente
                    .FirstOrDefault(r => r.Ruolo == RuoloUtente.Admin)?.Importo ?? 0,
                PercentualeAdmin = p.RicarichiUtente
                    .FirstOrDefault(r => r.Ruolo == RuoloUtente.Admin)?.Percentuale ?? 0,
                SelectedServizi = p.ServiziAggiuntivi.Select(s => s.Id).ToList(),
                MacroCategorie = await GetMacroCategorieAsync(),
                ServiziAggiuntivi = await GetServiziAsync(),
                ExistingDocumenti = p.DocumentiPolizza
                    .Select(d => new DocumentoPolizzaItemViewModel
                    {
                        Id = d.Id,
                        NomeFile = d.NomeFile,
                        Url = d.Url
                    })
                    .ToList()
            };

            return View(vm);
        }

        // POST: Polizze/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PolizzaFormViewModel vm)
        {
            if (vm.Id != id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                vm.MacroCategorie = await GetMacroCategorieAsync();
                vm.ServiziAggiuntivi = await GetServiziAsync();

                var docs = await _ctx.DocumentiPolizza
                    .Where(d => d.PolizzaId == id)
                    .Select(d => new DocumentoPolizzaItemViewModel
                    {
                        Id = d.Id,
                        NomeFile = d.NomeFile,
                        Url = d.Url
                    })
                    .ToListAsync();

                vm.ExistingDocumenti = docs;
                return View(vm);
            }

            var p = await _ctx.Polizze
                .Include(x => x.RicarichiUtente)
                .Include(x => x.ServiziAggiuntivi)
                .Include(x => x.DocumentiPolizza)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return NotFound();

            p.Nome = vm.Nome;
            p.Descrizione = vm.Descrizione;
            p.MacroCategoriaId = vm.MacroCategoriaId;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Challenge();

            var admin = p.RicarichiUtente.FirstOrDefault(r => r.Ruolo == RuoloUtente.Admin);
            if (admin == null)
            {
                admin = new RicaricoUtente
                {
                    UtenteId = userId,
                    PolizzaId = p.Id,
                    Ruolo = RuoloUtente.Admin
                };
                _ctx.RicarichiUtente.Add(admin);
            }

            admin.Importo = vm.ImportoAdmin;
            admin.Percentuale = vm.PercentualeAdmin;

            p.ServiziAggiuntivi.Clear();
            foreach (var sid in vm.SelectedServizi)
            {
                var servizio = await _ctx.ServiziAggiuntivi.FindAsync(sid);
                if (servizio != null)
                    p.ServiziAggiuntivi.Add(servizio);
            }

            if (vm.Documenti != null && vm.Documenti.Any())
            {
                foreach (var file in vm.Documenti.Where(f => f != null && f.Length > 0))
                {
                    var uploaded = await _storage.UploadAsync(
                        file.OpenReadStream(),
                        $"polizze/{p.Id}/{file.FileName}");

                    var doc = new DocumentoPolizza
                    {
                        PolizzaId = p.Id,
                        NomeFile = file.FileName,
                        Url = uploaded.ToString()
                    };

                    _ctx.DocumentiPolizza.Add(doc);
                }
            }

            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = p.Id });
        }

        // GET: Polizze/DownloadDocumento/5
        public async Task<IActionResult> DownloadDocumento(int id)
        {
            var doc = await _ctx.DocumentiPolizza
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doc == null)
                return NotFound();

            return Redirect(doc.Url);
        }

        // POST: Polizze/EliminaDocumento/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminaDocumento(int id, int polizzaId)
        {
            var doc = await _ctx.DocumentiPolizza
                .FirstOrDefaultAsync(d => d.Id == id && d.PolizzaId == polizzaId);

            if (doc == null)
                return NotFound();

            _ctx.DocumentiPolizza.Remove(doc);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = polizzaId });
        }

        // GET: Polizze/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _ctx.Polizze
                .Include(x => x.MacroCategoria)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return NotFound();

            var vm = new PolizzaGridViewModel
            {
                Id = p.Id,
                Nome = p.Nome,
                Categoria = p.MacroCategoria.Nome
            };

            return View(vm);
        }

        // POST: Polizze/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _ctx.Polizze.FindAsync(id);
            if (p != null)
            {
                _ctx.Polizze.Remove(p);
                await _ctx.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetMacroCategorieAsync()
        {
            var list = await _ctx.MacroCategorie
                .Select(m => new SelectListItem(m.Nome, m.Id.ToString()))
                .ToListAsync();

            list.Insert(0, new SelectListItem("-- Seleziona macrocategoria --", ""));
            return list;
        }

        private async Task<List<SelectListItem>> GetServiziAsync()
        {
            return await _ctx.ServiziAggiuntivi
                .Select(s => new SelectListItem(s.Nome, s.Id.ToString()))
                .ToListAsync();
        }
    }
}