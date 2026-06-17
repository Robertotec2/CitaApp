using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;
using Microsoft.Data.Sqlite;

namespace CitasApp.Infrastructure.Repositories.Sqlite;

/// <summary>
/// Adaptador de salida — implementa IPacienteRepository usando SQLite.
/// Comparte el mismo archivo .db que SqliteMedicoRepository y SqliteCitaRepository.
/// </summary>
public class SqlitePacienteRepository : IPacienteRepository
{
    private readonly string _connectionString;

    public SqlitePacienteRepository(string dataFolder)
    {
        var dbPath = Path.Combine(dataFolder, "citasapp.db");
        _connectionString = $"Data Source={dbPath}";
        InicializarTabla();
    }

    private void InicializarTabla()
    {
        try
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Pacientes (
                    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre   TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Email    TEXT,
                    Telefono TEXT
                );";
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException("No se pudo inicializar la tabla Pacientes.", ex);
        }
    }

    private static Paciente LeerFila(SqliteDataReader r) => new()
    {
        Id       = r.GetInt32(0),
        Nombre   = r.GetString(1),
        Apellido = r.GetString(2),
        Email    = r.IsDBNull(3) ? string.Empty : r.GetString(3),
        Telefono = r.IsDBNull(4) ? string.Empty : r.GetString(4)
    };

    public List<Paciente> ObtenerTodos()
    {
        try
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Apellido, Email, Telefono FROM Pacientes;";

            var lista = new List<Paciente>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(LeerFila(r));
            return lista;
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException("No se pudo leer la tabla Pacientes.", ex);
        }
    }

    public Paciente? ObtenerPorId(int id)
    {
        try
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Apellido, Email, Telefono FROM Pacientes WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var r = cmd.ExecuteReader();
            return r.Read() ? LeerFila(r) : null;
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException($"No se pudo leer el paciente con Id {id}.", ex);
        }
    }
}
