using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;

namespace CitasApp.Infrastructure.Repositories.Csv;

/// <summary>
/// Adaptador de salida — implementa IPacienteRepository leyendo pacientes.csv.
/// Formato: Id,Nombre,Apellido,Email,Telefono
/// </summary>
public class CsvPacienteRepository : IPacienteRepository
{
    private readonly string _filePath;

    public CsvPacienteRepository(string dataFolder)
    {
        _filePath = Path.Combine(dataFolder, "pacientes.csv");

        try
        {
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "Id,Nombre,Apellido,Email,Telefono\n");
        }
        catch (IOException ex)
        {
            throw new PersistenciaException("No se pudo inicializar pacientes.csv.", ex);
        }
    }

    private List<Paciente> LeerTodos()
    {
        try
        {
            var lista = new List<Paciente>();
            foreach (var linea in File.ReadAllLines(_filePath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                var p = linea.Split(',');
                if (p.Length < 5) continue;

                lista.Add(new Paciente
                {
                    Id       = int.Parse(p[0]),
                    Nombre   = p[1],
                    Apellido = p[2],
                    Email    = p[3],
                    Telefono = p[4]
                });
            }
            return lista;
        }
        catch (Exception ex) when (ex is IOException or FormatException)
        {
            throw new PersistenciaException("No se pudo leer pacientes.csv.", ex);
        }
    }

    public List<Paciente> ObtenerTodos() => LeerTodos();

    public Paciente? ObtenerPorId(int id) =>
        LeerTodos().FirstOrDefault(p => p.Id == id);
}
