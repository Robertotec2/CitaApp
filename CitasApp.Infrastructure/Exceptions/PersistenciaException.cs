namespace CitasApp.Infrastructure.Exceptions;

/// <summary>
/// Envuelve cualquier fallo de bajo nivel (archivo no encontrado, JSON corrupto,
/// CSV mal formado, error de SQLite, etc.) en una excepcion de infraestructura
/// legible, para que las capas superiores nunca vean tipos de System.IO o
/// Microsoft.Data.Sqlite directamente.
/// </summary>
public class PersistenciaException : Exception
{
    public PersistenciaException(string mensaje, Exception innerException)
        : base(mensaje, innerException)
    {
    }
}
