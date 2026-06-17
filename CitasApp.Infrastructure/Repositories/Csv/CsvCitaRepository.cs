using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;

namespace CitasApp.Infrastructure.Repositories.Csv;

/// <summary>
/// Adaptador de salida — implementa ICitaRepository leyendo y escribiendo citas.csv.
/// Formato: Id,PacienteId,MedicoId,Fecha(yyyy-MM-dd),Hora(HH:mm),Motivo,Estado
/// </summary>
public class CsvCitaRepository : ICitaRepository
{
    private readonly string _filePath;

    public CsvCitaRepository(string dataFolder)
    {
        _filePath = Path.Combine(dataFolder, "citas.csv");

        try
        {
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "Id,PacienteId,MedicoId,Fecha,Hora,Motivo,Estado\n");
        }
        catch (IOException ex)
        {
            throw new PersistenciaException("No se pudo inicializar citas.csv.", ex);
        }
    }

    private List<Cita> LeerTodos()
    {
        try
        {
            var lista = new List<Cita>();
            foreach (var linea in File.ReadAllLines(_filePath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                var p = linea.Split(',');
                if (p.Length < 7) continue;

                lista.Add(new Cita
                {
                    Id         = int.Parse(p[0]),
                    PacienteId = int.Parse(p[1]),
                    MedicoId   = int.Parse(p[2]),
                    Fecha      = DateOnly.ParseExact(p[3], "yyyy-MM-dd"),
                    Hora       = TimeOnly.ParseExact(p[4], "HH:mm"),
                    Motivo     = p[5],
                    Estado     = p[6]
                });
            }
            return lista;
        }
        catch (Exception ex) when (ex is IOException or FormatException)
        {
            throw new PersistenciaException("No se pudo leer citas.csv.", ex);
        }
    }

    private void EscribirTodos(List<Cita> citas)
    {
        try
        {
            var lineas = new List<string> { "Id,PacienteId,MedicoId,Fecha,Hora,Motivo,Estado" };

            foreach (var c in citas)
            {
                lineas.Add(string.Join(",",
                    c.Id,
                    c.PacienteId,
                    c.MedicoId,
                    c.Fecha.ToString("yyyy-MM-dd"),
                    c.Hora.ToString("HH:mm"),
                    Limpiar(c.Motivo),
                    Limpiar(c.Estado)));
            }

            File.WriteAllLines(_filePath, lineas);
        }
        catch (IOException ex)
        {
            throw new PersistenciaException("No se pudo escribir citas.csv.", ex);
        }
    }

    private static string Limpiar(string texto) => (texto ?? string.Empty).Replace(",", ";");

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
