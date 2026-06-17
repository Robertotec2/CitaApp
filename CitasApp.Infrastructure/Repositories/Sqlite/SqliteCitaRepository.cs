using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Exceptions;
using Microsoft.Data.Sqlite;

namespace CitasApp.Infrastructure.Repositories.Sqlite;

/// <summary>
/// Adaptador de salida — implementa ICitaRepository usando SQLite.
/// </summary>
public class SqliteCitaRepository : ICitaRepository
{
    private readonly string _connectionString;

    public SqliteCitaRepository(string dataFolder)
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
                CREATE TABLE IF NOT EXISTS Citas (
                    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                    PacienteId INTEGER NOT NULL,
                    MedicoId   INTEGER NOT NULL,
                    Fecha      TEXT    NOT NULL,
                    Hora       TEXT    NOT NULL,
                    Motivo     TEXT,
                    Estado     TEXT    NOT NULL DEFAULT 'Pendiente'
                );";
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException("No se pudo inicializar la tabla Citas.", ex);
        }
    }

    private SqliteConnection Conectar()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return conn;
    }

    private static Cita LeerFila(SqliteDataReader r) => new()
    {
        Id         = r.GetInt32(0),
        PacienteId = r.GetInt32(1),
        MedicoId   = r.GetInt32(2),
        Fecha      = DateOnly.ParseExact(r.GetString(3), "yyyy-MM-dd"),
        Hora       = TimeOnly.ParseExact(r.GetString(4), "HH:mm"),
        Motivo     = r.IsDBNull(5) ? string.Empty : r.GetString(5),
        Estado     = r.GetString(6)
    };

    public List<Cita> ObtenerTodos()
    {
        try
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT Id, PacienteId, MedicoId, Fecha, Hora, Motivo, Estado FROM Citas;";

            var lista = new List<Cita>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(LeerFila(r));
            return lista;
        }
        catch (Exception ex) when (ex is SqliteException or FormatException)
        {
            throw new PersistenciaException("No se pudo leer la tabla Citas.", ex);
        }
    }

    public Cita? ObtenerPorId(int id)
    {
        try
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT Id, PacienteId, MedicoId, Fecha, Hora, Motivo, Estado FROM Citas WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var r = cmd.ExecuteReader();
            return r.Read() ? LeerFila(r) : null;
        }
        catch (Exception ex) when (ex is SqliteException or FormatException)
        {
            throw new PersistenciaException($"No se pudo leer la cita con Id {id}.", ex);
        }
    }

    public List<Cita> ObtenerPorPaciente(int pacienteId)
    {
        try
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT Id, PacienteId, MedicoId, Fecha, Hora, Motivo, Estado FROM Citas WHERE PacienteId = $pid;";
            cmd.Parameters.AddWithValue("$pid", pacienteId);

            var lista = new List<Cita>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(LeerFila(r));
            return lista;
        }
        catch (Exception ex) when (ex is SqliteException or FormatException)
        {
            throw new PersistenciaException("No se pudo leer las citas del paciente.", ex);
        }
    }

    public List<Cita> ObtenerPorMedico(int medicoId)
    {
        try
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT Id, PacienteId, MedicoId, Fecha, Hora, Motivo, Estado FROM Citas WHERE MedicoId = $mid;";
            cmd.Parameters.AddWithValue("$mid", medicoId);

            var lista = new List<Cita>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(LeerFila(r));
            return lista;
        }
        catch (Exception ex) when (ex is SqliteException or FormatException)
        {
            throw new PersistenciaException("No se pudo leer las citas del medico.", ex);
        }
    }

    public Cita Agregar(Cita cita)
    {
        try
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Citas (PacienteId, MedicoId, Fecha, Hora, Motivo, Estado)
                VALUES ($pid, $mid, $fecha, $hora, $motivo, $estado);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$pid",    cita.PacienteId);
            cmd.Parameters.AddWithValue("$mid",    cita.MedicoId);
            cmd.Parameters.AddWithValue("$fecha",  cita.Fecha.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$hora",   cita.Hora.ToString("HH:mm"));
            cmd.Parameters.AddWithValue("$motivo", cita.Motivo ?? string.Empty);
            cmd.Parameters.AddWithValue("$estado", cita.Estado ?? "Pendiente");

            var nuevoId = (long)(cmd.ExecuteScalar() ?? 0L);
            cita.Id = (int)nuevoId;
            return cita;
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException("No se pudo guardar la cita.", ex);
        }
    }

    public bool ConfirmarCita(int id)
    {
        try
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Citas SET Estado = 'Confirmada' WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException($"No se pudo confirmar la cita con Id {id}.", ex);
        }
    }

    public bool Eliminar(int id)
    {
        try
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Citas WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqliteException ex)
        {
            throw new PersistenciaException($"No se pudo eliminar la cita con Id {id}.", ex);
        }
    }
}
