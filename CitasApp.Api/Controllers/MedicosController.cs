using CitasApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly MedicoService _service;

    public MedicosController(MedicoService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.ObtenerTodos());

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => Ok(_service.ObtenerPorId(id));
}
