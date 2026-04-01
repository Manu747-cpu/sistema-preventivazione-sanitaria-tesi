// Controllers/ClienteController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Infrastructure.Data;

[Authorize(Policy = "Customer")]
public class ClienteController : Controller
{
    private readonly AppDbContext _ctx;
    public ClienteController(AppDbContext ctx) => _ctx = ctx;

    // 1) Lista Macrocategorie
    public async Task<IActionResult> Categorie()
    {
        var cats = await _ctx.MacroCategorie.AsNoTracking().ToListAsync();
        return View(cats);
    }

    public async Task<IActionResult> Sottocategorie(int macroId)
    {
        var subs = await _ctx.SubCategorie
                              .Where(s => s.MacroCategoriaPolizzaId == macroId)
                              .AsNoTracking()
                              .ToListAsync();
        ViewBag.MacroId = macroId;
        return View(subs);
    }

    // 3) Tabella per subId
    public async Task<IActionResult> Tabella(int id)
    {
        var sub = await _ctx.SubCategorie
                            .Include(s => s.Colonne.OrderBy(c => c.Ordine))
                            .Include(s => s.Righe.OrderBy(r => r.Ordine))
                            .FirstOrDefaultAsync(s => s.Id == id);
        if (sub == null) return NotFound();
        return View(sub);
    }
}
