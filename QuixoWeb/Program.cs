using Microsoft.EntityFrameworkCore;
using QuixoWeb.Data;
using QuixoWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC / Razor
builder.Services.AddControllersWithViews();

// DbContext con SQL Server (usa el nombre que tengas en appsettings.json)
builder.Services.AddDbContext<QuixoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuixoConnection")));

// Servicios de dominio
builder.Services.AddScoped<IGameService, GameService>();

// Sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Opcional:
    // options.Cookie.SameSite = SameSiteMode.Lax;
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // opcional, útil durante el desarrollo
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Recomendado: habilitar sesión antes de autorización y de mapear rutas
app.UseSession();

// Si aplicas auth posteriormente
// app.UseAuthentication();
app.UseAuthorization();

// Rutas: específicas primero, default al final
app.MapControllerRoute(
    name: "game",
    pattern: "Game/{action=SelectMode}/{id?}",
    defaults: new { controller = "Game" });

app.MapControllerRoute(
    name: "stats",
    pattern: "Stats/{action=Index}",
    defaults: new { controller = "Stats" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Migraciones al inicio
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<QuixoDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al crear/actualizar la base de datos.");
    }
}

app.Run();