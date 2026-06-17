using CitasApp.Api.Middleware;
using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Infraestructura: elige el Adaptador segun appsettings ─────────────────
var dataFolder = Path.Combine(builder.Environment.ContentRootPath, "data");
builder.Services.AddCitasInfrastructure(builder.Configuration, dataFolder);

// ── 2. Casos de uso (Application) ─────────────────────────────────────────────
builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<ICalculadoraService, CalculadoraService>();

// ── 3. Adaptador de entrada: API REST + Swagger ───────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CitasApp API",
        Version = "v1",
        Description = "API REST construida con Arquitectura Hexagonal (Puertos y Adaptadores)."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware propio: traduce excepciones de Domain/Infrastructure en respuestas HTTP.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
