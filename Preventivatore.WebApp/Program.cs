using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Preventivatore.Core.Interfaces;
using Preventivatore.Core.Mapping;
using Preventivatore.Infrastructure.Data;
using Preventivatore.Infrastructure.Data.Models;
using Preventivatore.Infrastructure.Repositories;
using Preventivatore.Infrastructure.Services;
using Preventivatore.Infrastructure.UnitOfWork;
using System.Net.Http.Headers;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// 1) DbContext
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 2) Identity + UI + EF stores
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = true;

        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// 3) Cookie di Identity
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Identity/Account/Login";
    opts.AccessDeniedPath = "/Identity/Account/AccessDenied";
    opts.Cookie.Name = "Preventivatore.Auth";

    opts.Cookie.SameSite = SameSiteMode.Lax;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// 4) Policies (MVP: SOLO Customer/Admin)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Customer", p => p.RequireRole("Customer"));
    options.AddPolicy("Admin",    p => p.RequireRole("Admin"));

    options.AddPolicy("SoloCustomer", p => p.RequireRole("Customer"));
    options.AddPolicy("SoloAdmin",    p => p.RequireRole("Admin"));
});

// 5) HttpClient per API
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001/"
    );
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json")
    );
});

// 6) DI - UnitOfWork, Repo, Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISubCategoriaRepository, SubCategoriaRepository>();
builder.Services.AddScoped<IMacroCategoriaRepository, MacroCategoriaRepository>();
builder.Services.AddScoped<IMacroCategoriaService, MacroCategoriaService>();
builder.Services.AddScoped<IStorageService, FileStorageService>();

// 7) AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 8) MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// —— SEEDING RUOLI E UTENTI (MVP: SOLO Admin/Customer) ————————————————
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var roleName in new[] { "Admin", "Customer" })
    {
        if (!await roleMgr.RoleExistsAsync(roleName))
            await roleMgr.CreateAsync(new IdentityRole(roleName));
    }

    // cliente1 → Customer
    const string cliUser = "cliente1";
    const string cliEmail = "cliente1@example.com";
    const string cliPwd = "Cliente123!";
    if (await userMgr.FindByNameAsync(cliUser) == null)
    {
        var cliente = new ApplicationUser
        {
            UserName = cliUser,
            Email = cliEmail,
            EmailConfirmed = true
        };
        var r = await userMgr.CreateAsync(cliente, cliPwd);
        if (r.Succeeded)
            await userMgr.AddToRoleAsync(cliente, "Customer");
    }

    // admin1 → Admin
    const string admUser = "admin1";
    const string admEmail = "admin1@example.com";
    const string admPwd = "Admin123!";
    if (await userMgr.FindByNameAsync(admUser) == null)
    {
        var admin1 = new ApplicationUser
        {
            UserName = admUser,
            Email = admEmail,
            EmailConfirmed = true
        };
        var r3 = await userMgr.CreateAsync(admin1, admPwd);
        if (r3.Succeeded)
            await userMgr.AddToRoleAsync(admin1, "Admin");
    }
}
// ————————————————————————————————————————————————————————————————

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();