using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;
using Microsoft.Data.Sqlite;

namespace CitasApp.Infrastructure.Repositories.Sqlite;

/// <summary>
/// Adaptador de salida — implementa IMedicoRepository usando SQLite.
/// </summary>
public class SqliteMedicoRepository : IMedicoRepository
{
    private readonly string _connectionString;

    public SqliteMedicoRepository(string dataFolder)
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
                CREATE TABLE IF NOT EXISTS Medicos (
                    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre         TEXT NOT NULL,
                    Apellido       TEXT NOT NULL,
                    Especialidad   TEXT,
                    NumeroLicencia TEXT
                );";
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException("No se pudo inicializar la tabla Medicos.", ex);
        }
    }

    private static Medico LeerFila(SqliteDataReader r) => new()
    {
        Id             = r.GetInt32(0),
        Nombre         = r.GetString(1),
        Apellido       = r.GetString(2),
        Especialidad   = r.IsDBNull(3) ? string.Empty : r.GetString(3),
        NumeroLicencia = r.IsDBNull(4) ? string.Empty : r.GetString(4)
    };

    public List<Medico> ObtenerTodos()
    {
        try
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Apellido, Especialidad, NumeroLicencia FROM Medicos;";

            var lista = new List<Medico>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(LeerFila(r));
            return lista;
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException("No se pudo leer la tabla Medicos.", ex);
        }
    }

    public Medico? ObtenerPorId(int id)
    {
        try
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT Id, Nombre, Apellido, Especialidad, NumeroLicencia FROM Medicos WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var r = cmd.ExecuteReader();
            return r.Read() ? LeerFila(r) : null;
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException($"No se pudo leer el medico con Id {id}.", ex);
        }
    }
}
