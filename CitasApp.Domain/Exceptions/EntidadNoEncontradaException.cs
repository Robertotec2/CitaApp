namespace CitasApp.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se busca una entidad (Paciente, Medico, Cita) por un id que no
/// existe en el adaptador de persistencia activo, sin importar si ese adaptador
/// es Json, Csv o Sqlite.
/// </summary>
public class EntidadNoEncontradaException : Exception
{
    public EntidadNoEncontradaException(string entidad, int id)
        : base($"No se encontro {entidad} con Id = {id}.")
    {
    }

    public EntidadNoEncontradaException(string mensaje) : base(mensaje)
    {
    }
}
