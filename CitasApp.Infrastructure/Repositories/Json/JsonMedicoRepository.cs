using System.Text.Json;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;

namespace CitasApp.Infrastructure.Repositories.Json;

/// <summary>
/// Adaptador de salida — implementa IMedicoRepository leyendo medicos.json.
/// </summary>
public class JsonMedicoRepository : IMedicoRepository
{
    private readonly string _path;

    private static readonly JsonSerializerOptions Opciones = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonMedicoRepository(string dataFolder)
    {
        _path = Path.Combine(dataFolder, "medicos.json");
    }

    public List<Medico> ObtenerTodos()
    {
        try
        {
            if (!File.Exists(_path)) return new List<Medico>();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Medico>>(json, Opciones) ?? new List<Medico>();
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            throw new PersistenciaException("No se pudo leer medicos.json.", ex);
        }
    }

    private void EscribirTodos(List<Medico> medicos)
    {
        try
        {
            File.WriteAllText(_path, JsonSerializer.Serialize(medicos, Opciones));
        }
        catch (IOException ex)
        {
            throw new PersistenciaException("No se pudo escribir medicos.json.", ex);
        }
    }

    public Medico? ObtenerPorId(int id) =>
        ObtenerTodos().FirstOrDefault(m => m.Id == id);

    public Medico Agregar(Medico medico)
    {
        var medicos = ObtenerTodos();
        medico.Id = medicos.Count > 0 ? medicos.Max(m => m.Id) + 1 : 1;
        medicos.Add(medico);
        EscribirTodos(medicos);
        return medico;
    }
}
