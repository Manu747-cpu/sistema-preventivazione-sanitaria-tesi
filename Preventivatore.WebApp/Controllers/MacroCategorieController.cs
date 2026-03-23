using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;
using Preventivatore.Infrastructure.Data;

namespace Preventivatore.WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    public class MacroCategorieController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;

        public MacroCategorieController(AppDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        // GET: MacroCategorie
        public async Task<IActionResult> Index()
        {
            var list = await _context.MacroCategorie
                                     .AsNoTracking()
                                     .ToListAsync();
            return View(list);
        }

        // GET: MacroCategorie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var mc = await _context.MacroCategorie
                                   .Include(m => m.SubCategorie)
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(m => m.Id == id);

            if (mc == null) return NotFound();

            return View(mc);
        }

        // GET: MacroCategorie/Create
        public IActionResult Create() => View();

        // POST: MacroCategorie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nome,Descrizione")] MacroCategoriaPolizza model,
            IFormFile? ImmagineFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (ImmagineFile != null && ImmagineFile.Length > 0)
            {
                model.UrlImmagine = await _storageService.SaveFileAsync(
                    ImmagineFile.OpenReadStream(),
                    "macrocategorie",
                    ImmagineFile.FileName);
            }

            _context.MacroCategorie.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: MacroCategorie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mc = await _context.MacroCategorie.FindAsync(id);
            if (mc == null) return NotFound();

            return View(mc);
        }

        // POST: MacroCategorie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Nome,Descrizione")] MacroCategoriaPolizza model,
            IFormFile? ImmagineFile)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var entity = await _context.MacroCategorie.FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();

            entity.Nome = model.Nome;
            entity.Descrizione = model.Descrizione;

            if (ImmagineFile != null && ImmagineFile.Length > 0)
            {
                entity.UrlImmagine = await _storageService.SaveFileAsync(
                    ImmagineFile.OpenReadStream(),
                    "macrocategorie",
                    ImmagineFile.FileName);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: MacroCategorie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mc = await _context.MacroCategorie
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(m => m.Id == id);

            if (mc == null) return NotFound();

            return View(mc);
        }

        // POST: MacroCategorie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mc = await _context.MacroCategorie.FindAsync(id);
            if (mc != null)
            {
                _context.MacroCategorie.Remove(mc);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}