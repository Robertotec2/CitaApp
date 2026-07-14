using CitasApp.Api.DTOs;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly ICitaService _service;

    public CitasController(ICitaService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.ObtenerTodas());

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => Ok(_service.ObtenerPorId(id));

    [HttpGet("porpaciente/{pacienteId:int}")]
    public IActionResult PorPaciente(int pacienteId) => Ok(_service.ObtenerPorPaciente(pacienteId));

    /// <summary>POST /api/citas — valida paciente/medico/fecha en CitaService antes de guardar.</summary>
    [HttpPost]
    public IActionResult Crear([FromBody] CrearCitaDto dto)
    {
        var cita = new Cita
        {
            PacienteId = dto.PacienteId,
            MedicoId   = dto.MedicoId,
            Fecha      = dto.Fecha,
            Hora       = dto.Hora,
            Motivo     = dto.Motivo
        };

        var creada = _service.Agregar(cita);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    [HttpPatch("{id:int}/confirmar")]
    public IActionResult Confirmar(int id)
    {
        _service.ConfirmarCita(id);
        return NoContent();
    }
}
