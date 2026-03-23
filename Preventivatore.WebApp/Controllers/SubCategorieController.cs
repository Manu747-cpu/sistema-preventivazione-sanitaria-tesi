// Preventivatore.WebApp/Controllers/SubCategorieController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Infrastructure.Data;
using Preventivatore.WebApp.ViewModels;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Preventivatore.WebApp.Controllers
{

    [Authorize(Policy = "Admin")]

    public class SubCategorieController : Controller
    {
        private readonly AppDbContext _ctx;
        public SubCategorieController(AppDbContext ctx) => _ctx = ctx;

        // 1) INDEX
        public async Task<IActionResult> Index(int macroId)
        {
            ViewBag.MacroId = macroId;

            var list = await _ctx.SubCategorie
                .Where(s => s.MacroCategoriaPolizzaId == macroId)
                .Include(s => s.Colonne)
                .Include(s => s.Righe)
                .AsNoTracking()
                .ToListAsync();

            var vm = list.Select(s => new SubCategoriaIndexViewModel
            {
                Id = s.Id,
                Nome = s.Nome,
                Colonne = s.Colonne
                              .OrderBy(c => c.Ordine)
                              .Select(c => c.Intestazione)
                              .ToList(),
                Righe = s.Righe
                              .OrderBy(r => r.Ordine)
                              .Select(r => r.Label)
                              .ToList(),
                Celle = s.Righe
                              .OrderBy(r => r.Ordine)
                              .Select(r => JsonSerializer.Deserialize<List<string>>(r.CelleJson)!)
                              .ToList()
            })
            .ToList();

            return View(vm);
        }

        // 2) CREATE GET
        [HttpGet]
        public IActionResult Create(int macroId)
        {
            var vm = new SubCategoriaViewModel
            {
                MacroCategoriaPolizzaId = macroId,
                Colonne = new List<string> { "" },
                Righe = new List<string> { "" },
                Celle = new List<List<string>> { new List<string> { "" } }
            };
            return View(vm);
        }

        // 2) CREATE POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoriaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ricostruisco model.Celle dai campi del form
            var form = Request.Form;
            int rows = model.Righe.Count;
            int cols = model.Colonne.Count;
            model.Celle = new List<List<string>>(rows);
            for (int r = 0; r < rows; r++)
            {
                var rowList = new List<string>(cols);
                for (int c = 0; c < cols; c++)
                    rowList.Add(form[$"Celle[{r}][{c}]"].FirstOrDefault() ?? "");
                model.Celle.Add(rowList);
            }

            var ent = new SubCategoria
            {
                Nome = model.Nome,
                MacroCategoriaPolizzaId = model.MacroCategoriaPolizzaId,
                Colonne = model.Colonne
                              .Select((h, i) => new SubCategoriaColonna { Intestazione = h, Ordine = i })
                              .ToList(),
                Righe = model.Righe
                              .Select((label, i) => new SubCategoriaRiga
                              {
                                  Label = label,
                                  Ordine = i,
                                  CelleJson = JsonSerializer.Serialize(model.Celle[i])
                              })
                              .ToList()
            };

            _ctx.SubCategorie.Add(ent);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { macroId = ent.MacroCategoriaPolizzaId });
        }

        // 3) EDIT GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var e = await _ctx.SubCategorie
                .Include(s => s.Colonne)
                .Include(s => s.Righe)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (e == null) return NotFound();

            var vm = new SubCategoriaViewModel
            {
                Id = e.Id,
                Nome = e.Nome,
                MacroCategoriaPolizzaId = e.MacroCategoriaPolizzaId,
                Colonne = e.Colonne
                              .OrderBy(c => c.Ordine)
                              .Select(c => c.Intestazione)
                              .ToList(),
                Righe = e.Righe
                              .OrderBy(r => r.Ordine)
                              .Select(r => r.Label)
                              .ToList(),
                Celle = e.Righe
                              .OrderBy(r => r.Ordine)
                              .Select(r => JsonSerializer.Deserialize<List<string>>(r.CelleJson)!)
                              .ToList()
            };
            return View(vm);
        }

        // 3) EDIT POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoriaViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            // ricostruisco vm.Celle dai campi del form
            var form2 = Request.Form;
            int rows2 = vm.Righe.Count;
            int cols2 = vm.Colonne.Count;
            vm.Celle = new List<List<string>>(rows2);
            for (int r = 0; r < rows2; r++)
            {
                var rowList = new List<string>(cols2);
                for (int c = 0; c < cols2; c++)
                    rowList.Add(form2[$"Celle[{r}][{c}]"].FirstOrDefault() ?? "");
                vm.Celle.Add(rowList);
            }

            var e = await _ctx.SubCategorie
                .Include(s => s.Colonne)
                .Include(s => s.Righe)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (e == null) return NotFound();

            e.Nome = vm.Nome;

            // **rimuovi i vecchi figli dal contesto**
            _ctx.RemoveRange(e.Colonne);
            _ctx.RemoveRange(e.Righe);

            // **ricrea le nuove collection**
            e.Colonne = vm.Colonne
                          .Select((h, i) => new SubCategoriaColonna { Intestazione = h, Ordine = i })
                          .ToList();
            e.Righe = vm.Righe
                          .Select((label, i) => new SubCategoriaRiga
                          {
                              Label = label,
                              Ordine = i,
                              CelleJson = JsonSerializer.Serialize(vm.Celle[i])
                          })
                          .ToList();

            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { macroId = vm.MacroCategoriaPolizzaId });
        }

        // 4) DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _ctx.SubCategorie.FindAsync(id);
            if (e == null) return NotFound();
            return View(e);
        }

        // 4) DELETE POST
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var e = await _ctx.SubCategorie.FindAsync(id);
            if (e != null)
            {
                _ctx.SubCategorie.Remove(e);
                await _ctx.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { macroId = e.MacroCategoriaPolizzaId });
            }
            return NotFound();
        }
    }
}
