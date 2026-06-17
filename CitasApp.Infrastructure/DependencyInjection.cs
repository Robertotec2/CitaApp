using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Repositories.Csv;
using CitasApp.Infrastructure.Repositories.Json;
using CitasApp.Infrastructure.Repositories.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CitasApp.Infrastructure;

/// <summary>
/// Punto unico donde se conecta un Adaptador (Json, Csv o Sqlite) a los Puertos
/// del Dominio. El proveedor se elige por configuracion
/// ("Persistencia:Proveedor" en appsettings.json) sin tocar Domain ni Application,
/// y sin recompilar nada — exactamente el beneficio que promete la Arquitectura
/// Hexagonal.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCitasInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string dataFolder)
    {
        Directory.CreateDirectory(dataFolder);

        var proveedor = (configuration["Persistencia:Proveedor"] ?? "Json").Trim().ToLowerInvariant();

        switch (proveedor)
        {
            case "csv":
                services.AddScoped<IPacienteRepository>(_ => new CsvPacienteRepository(dataFolder));
                services.AddScoped<IMedicoRepository>(_   => new CsvMedicoRepository(dataFolder));
                services.AddScoped<ICitaRepository>(_     => new CsvCitaRepository(dataFolder));
                break;

            case "sqlite":
                services.AddScoped<IPacienteRepository>(_ => new SqlitePacienteRepository(dataFolder));
                services.AddScoped<IMedicoRepository>(_   => new SqliteMedicoRepository(dataFolder));
                services.AddScoped<ICitaRepository>(_     => new SqliteCitaRepository(dataFolder));
                break;

            case "json":
            default:
                services.AddScoped<IPacienteRepository>(_ => new JsonPacienteRepository(dataFolder));
                services.AddScoped<IMedicoRepository>(_   => new JsonMedicoRepository(dataFolder));
                services.AddScoped<ICitaRepository>(_     => new JsonCitaRepository(dataFolder));
                break;
        }

        return services;
    }
}
