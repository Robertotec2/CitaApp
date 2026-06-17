using CitasApp.Domain.Exceptions;
using CitasApp.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers;

public class CalculadoraController : Controller
{
    private readonly ICalculadoraService _calculadora;

    public CalculadoraController(ICalculadoraService calculadora)
    {
        _calculadora = calculadora;
    }

    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Calcular(double a, double b, string operacion)
    {
        try
        {
            double resultado = operacion switch
            {
                "sumar"        => _calculadora.Sumar(a, b),
                "restar"       => _calculadora.Restar(a, b),
                "multiplicar"  => _calculadora.Multiplicar(a, b),
                "dividir"      => _calculadora.Dividir(a, b),
                _              => throw new OperacionInvalidaException("Operacion no reconocida.")
            };

            ViewBag.Resultado = resultado;
        }
        catch (OperacionInvalidaException ex)
        {
            ViewBag.Error = ex.Message;
        }

        ViewBag.A          = a;
        ViewBag.B          = b;
        ViewBag.Operacion  = operacion;
        return View("Index");
    }
}
