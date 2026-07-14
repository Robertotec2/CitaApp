using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Infraestructura: mismo factory que usa CitasApp.Api ───────────────────
var dataFolder = Path.Combine(builder.Environment.ContentRootPath, "data");
builder.Services.AddCitasInfrastructure(builder.Configuration, dataFolder);

// ── 2. Casos de uso (Application) ─────────────────────────────────────────────
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<ICalculadoraService, CalculadoraService>();

// ── 3. Adaptador de entrada: MVC con vistas Cyberpunk ─────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
