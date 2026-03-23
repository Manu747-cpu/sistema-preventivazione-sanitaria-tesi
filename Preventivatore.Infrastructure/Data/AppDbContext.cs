using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Entities;
using Preventivatore.Infrastructure.Data.Models;


namespace Preventivatore.Infrastructure.Data
{
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // ■ DbSet delle entità esistenti
    public DbSet<MacroCategoriaPolizza> MacroCategorie { get; set; } = null!;
    public DbSet<Polizza> Polizze { get; set; } = null!;
    public DbSet<Preventivo> Preventivi { get; set; } = null!;
    public DbSet<PreventivoMvp> PreventiviMvp { get; set; } = null!;
    public DbSet<PreventivoFile> PreventivoFiles { get; set; } = null!;
    public DbSet<PreventivoServizioAggiuntivo> PreventivoServiziAggiuntivi { get; set; } = null!;
    public DbSet<DocumentoPreventivo> DocumentiPreventivo { get; set; } = null!;

    // ■ SubCategorie (UNA SOLA)
    public DbSet<Preventivatore.Core.Entities.SubCategoria> SubCategorie { get; set; } = null!;

    // ■ Nuovi DbSet
    public DbSet<ServizioAggiuntivo> ServiziAggiuntivi { get; set; } = null!;
    public DbSet<DocumentoPolizza> DocumentiPolizza { get; set; } = null!;
    public DbSet<RicaricoUtente> RicarichiUtente { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ■ Mapping MacroCategoriePolizza
            builder.Entity<MacroCategoriaPolizza>(eb =>
            {
                eb.ToTable("MacroCategoriePolizza");
                eb.HasKey(x => x.Id);
                eb.Property(x => x.Nome)
                  .IsRequired()
                  .HasMaxLength(100);
                eb.Property(x => x.Descrizione)
                  .HasMaxLength(500);
                eb.Property(x => x.UrlImmagine)
                  .HasMaxLength(200);

                eb.HasMany(x => x.Polizze)
                  .WithOne(p => p.MacroCategoria)
                  .HasForeignKey(p => p.MacroCategoriaId)
                  .OnDelete(DeleteBehavior.Cascade);

                eb.HasMany(x => x.SubCategorie)
                  .WithOne(s => s.MacroCategoriaPolizza)
                  .HasForeignKey(s => s.MacroCategoriaPolizzaId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            // ■ Mapping Polizza
            builder.Entity<Polizza>(eb =>
            {
                eb.ToTable("Polizze");
                eb.HasKey(x => x.Id);
                eb.Property(x => x.Nome)
                  .IsRequired()
                  .HasMaxLength(100);
                eb.Property(x => x.Descrizione)
                  .HasMaxLength(500);

                eb.HasOne(x => x.MacroCategoria)
                  .WithMany()
                  .HasForeignKey(x => x.MacroCategoriaId)
                  .OnDelete(DeleteBehavior.Cascade);

                // RELAZIONI con i nuovi due DbSet
                eb.HasMany(x => x.ServiziAggiuntivi)
                  .WithOne(s => s.Polizza)
                  .HasForeignKey(s => s.PolizzaId)
                  .OnDelete(DeleteBehavior.Cascade);

                eb.HasMany(x => x.DocumentiPolizza)
                  .WithOne(d => d.Polizza)
                  .HasForeignKey(d => d.PolizzaId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            // ■ Mapping ServizioAggiuntivo
            builder.Entity<ServizioAggiuntivo>(eb =>
            {
                eb.ToTable("ServiziAggiuntivi");
                eb.HasKey(x => x.Id);

                eb.Property(x => x.Nome)
                  .IsRequired()
                  .HasMaxLength(100);

                eb.Property(x => x.TipoImporto)
                  .IsRequired();

                eb.Property(x => x.Valore)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();
            });

            // ■ Mapping DocumentoPolizza
            builder.Entity<DocumentoPolizza>(eb =>
            {
                eb.ToTable("DocumentiPolizza");
                eb.HasKey(x => x.Id);

                eb.Property(x => x.NomeFile)
                  .IsRequired()
                  .HasMaxLength(200);

                eb.Property(x => x.Url)
                  .IsRequired()
                  .HasMaxLength(500);
            });

            // ■ Mapping RicaricoUtente
            builder.Entity<RicaricoUtente>(eb =>
            {
                eb.ToTable("RicarichiUtente");
                eb.HasKey(x => x.Id);

                eb.Property(x => x.UtenteId)
                  .IsRequired()
                  .HasMaxLength(450); // dimensione Id IdentityUser

                eb.Property(x => x.Percentuale)
                  .HasColumnType("decimal(5,2)")
                  .IsRequired();

                eb.Property(x => x.Importo)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

                eb.Property(x => x.Ruolo)
                  .IsRequired();
            });

            // ■ Mapping Preventivo
            builder.Entity<Preventivo>(eb =>
            {
                eb.ToTable("Preventivi");
                eb.HasKey(x => x.Id);
                eb.Property(x => x.UtenteId).IsRequired();
                eb.Property(x => x.PolizzaId).IsRequired();
                eb.Property(x => x.DataCreazione).IsRequired();
                eb.Property(x => x.TotaleFinale)
                  .HasColumnType("decimal(18,2)");
                eb.Property(x => x.RicaricoApplicato)
                  .HasColumnType("decimal(18,2)");

                eb.HasOne(x => x.Polizza)
                  .WithMany()
                  .HasForeignKey(x => x.PolizzaId);
            });

            // ■ Mapping PreventivoFile
            builder.Entity<PreventivoFile>(eb =>
            {
                eb.ToTable("PreventivoFiles");
                eb.HasKey(x => x.Id);
                eb.Property(x => x.PreventivoId).IsRequired();
                eb.Property(x => x.BlobName)
                  .IsRequired()
                  .HasMaxLength(200);
                eb.Property(x => x.Url)
                  .IsRequired()
                  .HasMaxLength(500);
                eb.Property(x => x.UploadedAt).IsRequired();
            });

            // ■ Mapping PreventivoServizioAggiuntivo
            builder.Entity<PreventivoServizioAggiuntivo>(eb =>
            {
                eb.ToTable("PreventivoServiziAggiuntivi");
                eb.HasKey(x => new { x.PreventivoId, x.ServizioId });
                eb.Property(x => x.ImportoCalcolato)
                  .HasColumnType("decimal(18,2)");

                eb.HasOne(x => x.Preventivo)
                  .WithMany(p => p.ServiziSelezionati)
                  .HasForeignKey(x => x.PreventivoId);
            });

            // ■ Mapping DocumentoPreventivo
            builder.Entity<DocumentoPreventivo>(eb =>
            {
                eb.ToTable("DocumentiPreventivo");
                eb.HasKey(x => x.Id);
                eb.Property(x => x.NomeFile)
                  .IsRequired()
                  .HasMaxLength(200);
                eb.Property(x => x.Url)
                  .IsRequired()
                  .HasMaxLength(500);
                eb.Property(x => x.CaricatoIl).IsRequired();

                eb.HasOne(x => x.Preventivo)
                  .WithMany(p => p.Documenti)
                  .HasForeignKey(x => x.PreventivoId);
            });

            // ■ Mapping SubCategoria (esistente)
            builder.Entity<SubCategoria>(eb =>
            {
                eb.ToTable("SubCategorie");
                eb.HasKey(x => x.Id);
                eb.Property(x => x.Nome)
                  .IsRequired()
                  .HasMaxLength(100);
                eb.Property(x => x.MacroCategoriaPolizzaId)
                  .IsRequired();

                eb.HasOne(x => x.MacroCategoriaPolizza)
                  .WithMany(m => m.SubCategorie)
                  .HasForeignKey(x => x.MacroCategoriaPolizzaId)
                  .OnDelete(DeleteBehavior.Cascade);

                eb.HasMany(x => x.Colonne)
                  .WithOne(c => c.SubCategoria)
                  .HasForeignKey(c => c.SubCategoriaId)
                  .OnDelete(DeleteBehavior.Cascade);

                eb.HasMany(x => x.Righe)
                  .WithOne(r => r.SubCategoria)
                  .HasForeignKey(r => r.SubCategoriaId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            // ■ Mapping SubCategoriaColonna (esistente)
        builder.Entity<Preventivatore.Core.Entities.SubCategoriaColonna>(eb =>
        {
            eb.ToTable("SubCategoriaColonna"); // <-- prima: SubCategorieColonna
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Intestazione)
              .IsRequired()
              .HasMaxLength(100);
            eb.Property(x => x.Ordine).IsRequired();
        });

        // ■ Mapping PreventivoMvp (MVP)
builder.Entity<PreventivoMvp>(eb =>
{
    eb.ToTable("PreventiviMvp");
    eb.HasKey(x => x.Id);

    eb.Property(x => x.UserId)
      .IsRequired()
      .HasMaxLength(450);

    eb.Property(x => x.SubCategoriaId).IsRequired();

    eb.Property(x => x.SubCategoriaNome)
      .IsRequired()
      .HasMaxLength(100);

    eb.Property(x => x.RowKey)
      .IsRequired()
      .HasMaxLength(100);

    eb.Property(x => x.ColKey)
      .IsRequired()
      .HasMaxLength(100);

    eb.Property(x => x.Value)
      .IsRequired()
      .HasMaxLength(200);

    eb.Property(x => x.Stato)
      .IsRequired()
      .HasMaxLength(30);

    eb.Property(x => x.DataCreazione).IsRequired();
});


            // ■ Mapping SubCategoriaRiga (esistente)
    builder.Entity<SubCategoriaRiga>(eb =>
    {
        eb.ToTable("SubCategoriaRiga"); // <-- prima: SubCategorieRiga
        eb.HasKey(x => x.Id);
        eb.Property(x => x.Ordine).IsRequired();
        eb.Property(x => x.CelleJson)
          .IsRequired()
          .HasColumnType("nvarchar(max)");

        eb.Property(x => x.Label)
          .IsRequired()
          .HasMaxLength(100);
    });

        }
    }
}
