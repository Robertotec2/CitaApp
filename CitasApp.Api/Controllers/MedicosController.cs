using CitasApp.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _service;

    public MedicosController(IMedicoService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.ObtenerTodos());

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => Ok(_service.ObtenerPorId(id));
}
