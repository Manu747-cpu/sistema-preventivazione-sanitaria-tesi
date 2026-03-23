using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Preventivatore.Infrastructure.Data;
using Preventivatore.WebApp.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;

namespace Preventivatore.WebApp.Controllers
{
    [Authorize(Policy = "Customer")]
    public class PreventiviController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _configuration;

        public PreventiviController(AppDbContext ctx, IConfiguration configuration)
        {
            _ctx = ctx;
            _configuration = configuration;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromSelection(int subCategoriaId, string rowKey, string colKey, string value)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var subNome = await _ctx.SubCategorie
                .Where(s => s.Id == subCategoriaId)
                .Select(s => s.Nome)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(subNome))
                return BadRequest("Sottocategoria non valida.");

            var p = new Core.Entities.PreventivoMvp
            {
                UserId = userId,
                SubCategoriaId = subCategoriaId,
                SubCategoriaNome = subNome,
                RowKey = rowKey,
                ColKey = colKey,
                Value = value
            };

            _ctx.PreventiviMvp.Add(p);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Miei));
        }

        [HttpGet]
        public async Task<IActionResult> Miei()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var lista = await _ctx.PreventiviMvp
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DataCreazione)
                .ToListAsync();

            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Dettagli(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _ctx.PreventiviMvp
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null)
                return NotFound();

            var subInfo = await _ctx.SubCategorie
                .AsNoTracking()
                .Where(s => s.Id == item.SubCategoriaId)
                .Select(s => new
                {
                    s.Id,
                    s.Nome,
                    MacroCategoriaId = s.MacroCategoriaPolizzaId,
                    MacroCategoriaNome = s.MacroCategoriaPolizza.Nome
                })
                .FirstOrDefaultAsync();

            var documenti = new List<PreventivoDocumentoViewModel>();

            if (subInfo != null)
            {
                documenti = await _ctx.DocumentiPolizza
                    .AsNoTracking()
                    .Where(d => d.Polizza.MacroCategoriaId == subInfo.MacroCategoriaId)
                    .OrderBy(d => d.Polizza.Nome)
                    .ThenBy(d => d.NomeFile)
                    .Select(d => new PreventivoDocumentoViewModel
                    {
                        Id = d.Id,
                        NomeFile = d.NomeFile,
                        Url = d.Url,
                        NomePolizza = d.Polizza.Nome
                    })
                    .ToListAsync();
            }

            var vm = new PreventivoDettaglioViewModel
            {
                Id = item.Id,
                UserId = item.UserId,
                SubCategoriaId = item.SubCategoriaId,
                SubCategoriaNome = item.SubCategoriaNome,
                RowKey = item.RowKey,
                ColKey = item.ColKey,
                Value = item.Value,
                DataCreazione = item.DataCreazione,
                Stato = item.Stato,
                MacroCategoriaNome = subInfo?.MacroCategoriaNome,
                Documenti = documenti
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocumento(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var doc = await _ctx.DocumentiPolizza
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doc == null)
                return NotFound();

            return Redirect(doc.Url);
        }

        [HttpGet]
        public async Task<IActionResult> ScaricaRiepilogo(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _ctx.PreventiviMvp
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null)
                return NotFound();

            var subInfo = await _ctx.SubCategorie
                .AsNoTracking()
                .Where(s => s.Id == item.SubCategoriaId)
                .Select(s => new
                {
                    s.Nome,
                    MacroCategoria = s.MacroCategoriaPolizza.Nome,
                    MacroCategoriaId = s.MacroCategoriaPolizzaId
                })
                .FirstOrDefaultAsync();

            var docs = new List<PreventivoDocumentoViewModel>();

            if (subInfo != null)
            {
                docs = await _ctx.DocumentiPolizza
                    .AsNoTracking()
                    .Where(d => d.Polizza.MacroCategoriaId == subInfo.MacroCategoriaId)
                    .OrderBy(d => d.Polizza.Nome)
                    .ThenBy(d => d.NomeFile)
                    .Select(d => new PreventivoDocumentoViewModel
                    {
                        Id = d.Id,
                        NomeFile = d.NomeFile,
                        Url = d.Url,
                        NomePolizza = d.Polizza.Nome
                    })
                    .ToListAsync();
            }

            var companyName = _configuration["Company:Name"] ?? "Preventivatore";
            var companySubtitle = _configuration["Company:Subtitle"] ?? "";
            var fileName = $"riepilogo_preventivo_{item.Id}.pdf";

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(header =>
                    {
                        header.Spacing(4);

                        header.Item()
                            .Text(companyName)
                            .FontSize(20)
                            .SemiBold()
                            .FontColor(Colors.Blue.Darken2);

                        if (!string.IsNullOrWhiteSpace(companySubtitle))
                        {
                            header.Item()
                                .Text(companySubtitle)
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken1);
                        }

                        header.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingVertical(18).Column(col =>
                    {
                        col.Spacing(14);

                        col.Item()
                            .Text("Riepilogo preventivo")
                            .FontSize(18)
                            .SemiBold();

                        col.Item().Background(Colors.Grey.Lighten4).Padding(12).Column(box =>
                        {
                            box.Spacing(6);

                            box.Item().Text($"ID preventivo: {item.Id}");
                            box.Item().Text($"Macrocategoria: {subInfo?.MacroCategoria ?? "Non disponibile"}");
                            box.Item().Text($"Sottocategoria: {subInfo?.Nome ?? item.SubCategoriaNome}");
                            box.Item().Text($"Riga selezionata: {item.RowKey}");
                            box.Item().Text($"Colonna selezionata: {item.ColKey}");
                            box.Item().Text($"Valore selezionato: {item.Value}");
                            box.Item().Text($"Data creazione: {item.DataCreazione:dd/MM/yyyy HH:mm:ss}");
                            box.Item().Text($"Stato: {item.Stato}");
                        });

                        col.Item().Text("Documenti disponibili").FontSize(14).SemiBold();

                        if (docs.Any())
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellHeader).Text("Nome file");
                                    header.Cell().Element(CellHeader).Text("Polizza");
                                });

                                foreach (var doc in docs)
                                {
                                    table.Cell().Element(CellBody).Text(doc.NomeFile);
                                    table.Cell().Element(CellBody).Text(doc.NomePolizza ?? "");
                                }

                                static IContainer CellHeader(IContainer container) =>
                                    container
                                        .Background(Colors.Blue.Lighten4)
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .Padding(8)
                                        .DefaultTextStyle(x => x.SemiBold());

                                static IContainer CellBody(IContainer container) =>
                                    container
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8);
                            });
                        }
                        else
                        {
                            col.Item()
                                .Background(Colors.Grey.Lighten4)
                                .Padding(10)
                                .Text("Nessun documento disponibile per questa categoria.");
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Documento generato automaticamente - ");
                            x.Span(companyName).SemiBold();
                        });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Elimina(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _ctx.PreventiviMvp
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null)
                return NotFound();

            _ctx.PreventiviMvp.Remove(item);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Miei));
        }
    }
}