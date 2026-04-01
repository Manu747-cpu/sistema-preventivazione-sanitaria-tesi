// Controllers/RamiController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Infrastructure.Data;
using Preventivatore.Core.Entities;
using Preventivatore.WebApp.ViewModels;
using System.Linq;
using System.Text.Json;

[Authorize(Policy = "Customer")]
public class RamiController : Controller
{
    private readonly AppDbContext _ctx;
    public RamiController(AppDbContext ctx) => _ctx = ctx;

    // 2.1) Home cliente
    public IActionResult Index()
    {
        return View();  
    }

    // 2.2) Elenco macrocategorie
    public async Task<IActionResult> Macrocategorie()
    {
        var items = await _ctx.MacroCategorie
                              .AsNoTracking()
                              .ToListAsync();
        return View(items);
    }

    // 2.3) Elenco sottocategorie
    public async Task<IActionResult> Sottocategorie(int macroId)
    {
        ViewBag.MacroId = macroId;
        var items = await _ctx.SubCategorie
                              .Where(s => s.MacroCategoriaPolizzaId == macroId)
                              .AsNoTracking()
                              .ToListAsync();
        return View(items);
    }

    // 2.4) Tabella dei valori e selezione
    public async Task<IActionResult> Tabella(int subId)
    {
        var s = await _ctx.SubCategorie
                          .Include(x => x.Colonne)
                          .Include(x => x.Righe)
                          .FirstOrDefaultAsync(x => x.Id == subId);
        if (s == null) return NotFound();

        var vm = new SubCategoriaIndexViewModel
        {
            Id = s.Id,
            Nome = s.Nome,
            Colonne = s.Colonne.OrderBy(c => c.Ordine).Select(c => c.Intestazione).ToList(),
            Righe = s.Righe.OrderBy(r => r.Ordine).Select(r => r.Label).ToList(),
            Celle = s.Righe.OrderBy(r => r.Ordine)
                       .Select(r => JsonSerializer.Deserialize<List<string>>(r.CelleJson)!)
                       .ToList()
        };
        return View(vm);
    }

    // 2.5) [opzionale] Azione per gestire la cella selezionata
    [HttpPost]
    public IActionResult SelezionaValore(int subId, string selectedCell)
    {
        // selectedCell = "r;c"
        var parts = selectedCell.Split(';').Select(int.Parse).ToArray();
        int r = parts[0], c = parts[1];
        // salva in sessione o DB la scelta dell'utente...
        return RedirectToAction("Conferma", new { subId, r, c });
    }

    public IActionResult Conferma(int subId, int r, int c)
    {
        ViewBag.Message = $"Hai scelto riga {r + 1}, colonna {c + 1}";
        return View();
    }
}
