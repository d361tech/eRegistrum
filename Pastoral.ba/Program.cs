using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pastoral.ba.Data;
using Pastoral.ba.Models;

var builder = WebApplication.CreateBuilder(args);

// Konekcija na bazu podataka
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registracija Identity servisa sa prilagođenim korisničkim modelom
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Onemogućena obavezna potvrda email-a za razvojnu fazu
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6; // Minimalna dužina lozinke
})
                .AddRoles<IdentityRole>() // Dodavanje podrške za role
                .AddEntityFrameworkStores<ApplicationDbContext>(); // Koristi ApplicationDbContext za Identity

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware konfiguracija
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Omogućavanje HTTPS-a i statičkih fajlova
app.UseHttpsRedirection();
app.UseStaticFiles();

// Routing i autentifikacija
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Konfiguracija ruta za kontrolere i Razor Pages
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Omogućava ugrađene Razor Pages za Identity

// SeedData inicijalizacija za role
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Pokretanje inicijalizacije podataka
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Greška pri inicijalizaciji podataka u bazi.");
    }
}

// Pokretanje aplikacije
app.Run();