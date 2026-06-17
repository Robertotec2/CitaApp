using CitasApp.Application.Services;
using CitasApp.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers;

public class PacienteController : Controller
{
    private readonly PacienteService _service;

    public PacienteController(PacienteService service)
    {
        _service = service;
    }

    public IActionResult Index() => View(_service.ObtenerTodos());

    public IActionResult Detalle(int id)
    {
        try
        {
            return View(_service.ObtenerPorId(id));
        }
        catch (EntidadNoEncontradaException)
        {
            return NotFound();
        }
    }
}
