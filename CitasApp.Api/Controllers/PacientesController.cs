using CitasApp.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    /// <summary>GET /api/pacientes</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(_service.ObtenerTodos());

    /// <summary>GET /api/pacientes/{id} — 404 automatico via middleware si no existe.</summary>
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => Ok(_service.ObtenerPorId(id));
}
