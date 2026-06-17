using CitasApp.Domain.Exceptions;
using CitasApp.Domain.Interfaces;

namespace CitasApp.Application.Services;

/// <summary>
/// Implementacion del puerto ICalculadoraService. No depende de Infrastructure
/// porque no necesita persistencia — es logica de dominio pura.
/// </summary>
public class CalculadoraService : ICalculadoraService
{
    public double Sumar(double a, double b)       => Math.Round(a + b, 10);
    public double Restar(double a, double b)      => Math.Round(a - b, 10);
    public double Multiplicar(double a, double b) => Math.Round(a * b, 10);

    public double Dividir(double a, double b)
    {
        if (b == 0)
            throw new OperacionInvalidaException("No se puede dividir entre cero.");

        return a / b;
    }
}
