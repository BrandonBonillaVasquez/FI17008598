
using Transportation.Interfaces;
using Transportation.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Registrar implementaciones con clave (Keyed Services) en .NET 8
builder.Services.AddKeyedTransient<IAirplanes, Airbus>("airbus");
builder.Services.AddKeyedTransient<IAirplanes, Boeing>("boeing");

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//Fuente: Copilot