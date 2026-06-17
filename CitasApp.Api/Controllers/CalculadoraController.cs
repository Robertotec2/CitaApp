using CitasApp.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculadoraController : ControllerBase
{
    private readonly ICalculadoraService _calculadora;

    public CalculadoraController(ICalculadoraService calculadora)
    {
        _calculadora = calculadora;
    }

    [HttpGet("sumar")]
    public IActionResult Sumar(double a, double b) => Ok(_calculadora.Sumar(a, b));

    [HttpGet("restar")]
    public IActionResult Restar(double a, double b) => Ok(_calculadora.Restar(a, b));

    [HttpGet("multiplicar")]
    public IActionResult Multiplicar(double a, double b) => Ok(_calculadora.Multiplicar(a, b));

    /// <summary>Division entre cero lanza OperacionInvalidaException -> 400 via middleware.</summary>
    [HttpGet("dividir")]
    public IActionResult Dividir(double a, double b) => Ok(_calculadora.Dividir(a, b));
}
