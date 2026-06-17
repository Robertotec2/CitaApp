namespace CitasApp.Domain.Interfaces;

/// <summary>
/// Puerto de entrada (Input Port / Use Case) para las operaciones aritmeticas
/// del modulo Calculadora, heredado de la rama Api-Calculadora.
/// </summary>
public interface ICalculadoraService
{
    double Sumar(double a, double b);
    double Restar(double a, double b);
    double Multiplicar(double a, double b);
    double Dividir(double a, double b);
}
