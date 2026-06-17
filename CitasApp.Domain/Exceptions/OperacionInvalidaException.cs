namespace CitasApp.Domain.Exceptions;

/// <summary>
/// Se lanza cuando una operacion de negocio recibe datos que violan una regla
/// del dominio (division entre cero, motivo vacio, fecha invalida, etc.).
/// </summary>
public class OperacionInvalidaException : Exception
{
    public OperacionInvalidaException(string mensaje) : base(mensaje)
    {
    }
}
