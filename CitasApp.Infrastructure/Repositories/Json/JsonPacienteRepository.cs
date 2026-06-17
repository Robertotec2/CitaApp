using System.Text.Json;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;

namespace CitasApp.Infrastructure.Repositories.Json;

/// <summary>
/// Adaptador de salida — implementa IPacienteRepository leyendo pacientes.json.
/// </summary>
public class JsonPacienteRepository : IPacienteRepository
{
    private readonly string _path;

    private static readonly JsonSerializerOptions Opciones = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonPacienteRepository(string dataFolder)
    {
        _path = Path.Combine(dataFolder, "pacientes.json");
    }

    public List<Paciente> ObtenerTodos()
    {
        try
        {
            if (!File.Exists(_path)) return new List<Paciente>();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Paciente>>(json, Opciones) ?? new List<Paciente>();
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            throw new PersistenciaException("No se pudo leer pacientes.json.", ex);
        }
    }

    public Paciente? ObtenerPorId(int id) =>
        ObtenerTodos().FirstOrDefault(p => p.Id == id);
}
