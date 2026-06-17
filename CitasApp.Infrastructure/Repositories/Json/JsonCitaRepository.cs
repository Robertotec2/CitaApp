using System.Text.Json;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;

namespace CitasApp.Infrastructure.Repositories.Json;

/// <summary>
/// Adaptador de salida — implementa ICitaRepository leyendo y escribiendo citas.json.
/// DateOnly/TimeOnly se serializan de forma nativa (soportado desde System.Text.Json 7+).
/// </summary>
public class JsonCitaRepository : ICitaRepository
{
    private readonly string _path;

    private static readonly JsonSerializerOptions Opciones = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonCitaRepository(string dataFolder)
    {
        _path = Path.Combine(dataFolder, "citas.json");
    }

    private List<Cita> LeerTodos()
    {
        try
        {
            if (!File.Exists(_path)) return new List<Cita>();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Cita>>(json, Opciones) ?? new List<Cita>();
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            throw new PersistenciaException("No se pudo leer citas.json.", ex);
        }
    }

    private void EscribirTodos(List<Cita> citas)
    {
        try
        {
            var json = JsonSerializer.Serialize(citas, Opciones);
            File.WriteAllText(_path, json);
        }
        catch (IOException ex)
        {
            throw new PersistenciaException("No se pudo escribir citas.json.", ex);
        }
    }

    public List<Cita> ObtenerTodos() => LeerTodos();

    public Cita? ObtenerPorId(int id) =>
        LeerTodos().FirstOrDefault(c => c.Id == id);

    public List<Cita> ObtenerPorPaciente(int pacienteId) =>
        LeerTodos().Where(c => c.PacienteId == pacienteId).ToList();

    public Cita Agregar(Cita cita)
    {
        var citas = LeerTodos();
        cita.Id = citas.Count > 0 ? citas.Max(c => c.Id) + 1 : 1;
        citas.Add(cita);
        EscribirTodos(citas);
        return cita;
    }

    public bool ConfirmarCita(int id)
    {
        var citas = LeerTodos();
        var cita  = citas.FirstOrDefault(c => c.Id == id);
        if (cita is null) return false;

        cita.Estado = "Confirmada";
        EscribirTodos(citas);
        return true;
    }
}
