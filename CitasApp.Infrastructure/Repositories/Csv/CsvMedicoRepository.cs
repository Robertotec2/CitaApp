using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;

namespace CitasApp.Infrastructure.Repositories.Csv;

/// <summary>
/// Adaptador de salida — implementa IMedicoRepository leyendo medicos.csv.
/// Formato: Id,Nombre,Apellido,Especialidad,NumeroLicencia
/// </summary>
public class CsvMedicoRepository : IMedicoRepository
{
    private readonly string _filePath;

    public CsvMedicoRepository(string dataFolder)
    {
        _filePath = Path.Combine(dataFolder, "medicos.csv");

        try
        {
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "Id,Nombre,Apellido,Especialidad,NumeroLicencia\n");
        }
        catch (IOException ex)
        {
            throw new PersistenciaException("No se pudo inicializar medicos.csv.", ex);
        }
    }

    private List<Medico> LeerTodos()
    {
        try
        {
            var lista = new List<Medico>();
            foreach (var linea in File.ReadAllLines(_filePath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                var p = linea.Split(',');
                if (p.Length < 5) continue;

                lista.Add(new Medico
                {
                    Id             = int.Parse(p[0]),
                    Nombre         = p[1],
                    Apellido       = p[2],
                    Especialidad   = p[3],
                    NumeroLicencia = p[4]
                });
            }
            return lista;
        }
        catch (Exception ex) when (ex is IOException or FormatException)
        {
            throw new PersistenciaException("No se pudo leer medicos.csv.", ex);
        }
    }

    private void EscribirTodos(List<Medico> medicos)
    {
        try
        {
            var lineas = new List<string> { "Id,Nombre,Apellido,Especialidad,NumeroLicencia" };
            foreach (var m in medicos)
                lineas.Add(string.Join(",",
                    m.Id,
                    Limpiar(m.Nombre),
                    Limpiar(m.Apellido),
                    Limpiar(m.Especialidad),
                    Limpiar(m.NumeroLicencia)));
            File.WriteAllLines(_filePath, lineas);
        }
        catch (IOException ex)
        {
            throw new PersistenciaException("No se pudo escribir medicos.csv.", ex);
        }
    }

    private static string Limpiar(string texto) => (texto ?? string.Empty).Replace(",", ";");

    public List<Medico> ObtenerTodos() => LeerTodos();

    public Medico? ObtenerPorId(int id) =>
        LeerTodos().FirstOrDefault(m => m.Id == id);

    public Medico Agregar(Medico medico)
    {
        var medicos = LeerTodos();
        medico.Id = medicos.Count > 0 ? medicos.Max(m => m.Id) + 1 : 1;
        medicos.Add(medico);
        EscribirTodos(medicos);
        return medico;
    }
}
