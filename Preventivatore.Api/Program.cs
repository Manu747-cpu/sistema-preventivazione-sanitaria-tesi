// Preventivatore.Api/Program.cs

using AspNetCoreRateLimit;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Preventivatore.Core.Interfaces;
using Preventivatore.Core.Mapping;
using Preventivatore.Core.Settings;
using Preventivatore.Infrastructure.Data;
using Preventivatore.Infrastructure.Data.Models;
using Preventivatore.Infrastructure.Repositories;
using Preventivatore.Infrastructure.Services;
using Preventivatore.Infrastructure.UnitOfWork;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// (se JwtSettings non serve più, può essere tolto. Per ora lo lasciamo perché non dà fastidio)
builder.Services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

// Porta API
builder.WebHost.UseUrls("http://localhost:5000");

// ─── 1) DbContext ───────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection"))
);

// ─── 2) AutoMapper ──────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// ─── 3) Identity (cookie) ───────────────────────────────────────────────────────
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(opts =>
    {
        opts.Password.RequireDigit = true;
        opts.Password.RequireLowercase = true;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Account/Login";
    opts.AccessDeniedPath = "/Account/AccessDenied";
    // opzionale: puoi dare un nome cookie diverso per evitare collisioni con WebApp
    // opts.Cookie.Name = "Preventivatore.Api.Auth";
});

// ─── 4) Authorization Policies (MVP: Admin/Customer) ────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
    options.AddPolicy("Customer", p => p.RequireRole("Customer"));

    // alias per compatibilità (se qualche controller usa ancora questi nomi)
    options.AddPolicy("SoloAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("SoloCustomer", p => p.RequireRole("Customer"));
});

// ─── 5) Rate limiting ────────────────────────────────────────────────────────────
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ─── 6) Storage (Azure Blob) ─────────────────────────────────────────────────────
builder.Services.Configure<StorageSettings>(config.GetSection("Storage"));
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<StorageSettings>>().Value;
    return new BlobServiceClient(settings.ConnectionString);
});
builder.Services.AddScoped<IStorageService, AzureBlobStorageService>();

// ─── 7) DI - Repositories / Services / UnitOfWork (NO DUPLICATI) ─────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IMacroCategoriaRepository, MacroCategoriaRepository>();
builder.Services.AddScoped<IMacroCategoriaService, MacroCategoriaService>();

builder.Services.AddScoped<ISubCategoriaRepository, SubCategoriaRepository>();
builder.Services.AddScoped<ISubCategoriaService, SubCategoriaService>();

builder.Services.AddScoped<IPolizzaRepository, PolizzaRepository>();
builder.Services.AddScoped<IPolizzaService, PolizzaService>();

builder.Services.AddScoped<IPreventivoRepository, EfPreventivoRepository>();
builder.Services.AddScoped<IPreventivoService, PreventivoService>();

builder.Services.AddScoped<IPreventivoFileService, PreventivoFileService>();

// ─── 8) MVC / API configuration ─────────────────────────────────────────────────
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opts =>
    {
        opts.InvalidModelStateResponseFactory = ctx =>
        {
            var details = new ValidationProblemDetails(ctx.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Errori di validazione nei dati inviati."
            };
            return new BadRequestObjectResult(details);
        };
    });

// ─── 9) Swagger ────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema di preventivazione sanitaria API",
        Version = "v1"
    });

    c.TagActionsBy(api =>
    {
        if (api.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller))
        {
            return new[]
            {
                controller switch
                {
                    "Auth" => "Autenticazione",
                    "Files" => "Documenti",
                    "MacroCategorie" => "Categorie professionali",
                    "Polizze" => "Coperture professionali",
                    "Preventivo" => "Preventivi",
                    "PreventivoFiles" => "Documenti preventivo",
                    "SubCategorie" => "Coperture accessorie",
                    _ => controller
                }
            };
        }

        return new[] { "API" };
    });

    c.CustomSchemaIds(type =>
    {
        return type.Name switch
        {
            "CreatePolizzaDto" => "CreateCoperturaProfessionaleDto",
            "UpdatePolizzaDto" => "UpdateCoperturaProfessionaleDto",
            "MacroCategoriaDto" => "CategoriaProfessionaleDto",
            "SubCategoriaDto" => "CoperturaAccessoriaDto",
            "CreatePreventivoDto" => "CreatePreventivoDto",
            "PreventivoDto" => "PreventivoDto",
            "PreventivoFileDto" => "DocumentoPreventivoDto",
            "RegisterDto" => "RegisterDto",
            "LoginDto" => "LoginDto",
            "ProblemDetails" => "ProblemDetails",
            _ => type.Name
        };
    });

    c.DocumentFilter<SanitarySwaggerPathDocumentFilter>();
});

// ─── 10) Compression / caching / healthchecks ───────────────────────────────────
builder.Services.AddResponseCompression();
builder.Services.AddResponseCaching();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("Database");

// ─── 11) CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
        policy.WithOrigins(
                "http://localhost:5288", "https://localhost:5288",
                "http://localhost:5289", "https://localhost:5289",
                "http://localhost:5273", "https://localhost:5273"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

var app = builder.Build();

// ─── Seed ruoli (MVP: SOLO Admin/Customer) ──────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "Customer" })
    {
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));
    }
}

// ─── Middleware pipeline ────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DocumentTitle = "Sistema di preventivazione sanitaria API";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema di preventivazione sanitaria API V1");
    c.RoutePrefix = string.Empty;
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseResponseCompression();
app.UseResponseCaching();

app.UseRouting();

app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public sealed class SanitarySwaggerPathDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var updatedPaths = new OpenApiPaths();

        foreach (var path in swaggerDoc.Paths)
        {
            var newKey = path.Key
                .Replace("/api/MacroCategorie", "/api/CategorieProfessionali")
                .Replace("/api/Polizze", "/api/CopertureProfessionali")
                .Replace("/api/SubCategorie", "/api/CopertureAccessorie");

            updatedPaths.Add(newKey, path.Value);
        }

        swaggerDoc.Paths.Clear();

        foreach (var path in updatedPaths)
        {
            swaggerDoc.Paths.Add(path.Key, path.Value);
        }
    }
}