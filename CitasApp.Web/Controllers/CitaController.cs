using CitasApp.Domain.Exceptions;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers;

public class CitaController : Controller
{
    private readonly ICitaService     _service;
    private readonly IPacienteService _pacienteService;
    private readonly IMedicoService   _medicoService;

    public CitaController(ICitaService service, IPacienteService pacienteService, IMedicoService medicoService)
    {
        _service         = service;
        _pacienteService = pacienteService;
        _medicoService   = medicoService;
    }

    private void CargarListasParaFormulario()
    {
        ViewBag.Pacientes = _pacienteService.ObtenerTodos();
        ViewBag.Medicos   = _medicoService.ObtenerTodos();
    }

    public IActionResult Index()
    {
        CargarListasParaFormulario();
        return View(_service.ObtenerTodas());
    }

    public IActionResult PorPaciente(int pacienteId)
    {
        try
        {
            CargarListasParaFormulario();
            return View(_service.ObtenerPorPaciente(pacienteId));
        }
        catch (EntidadNoEncontradaException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        CargarListasParaFormulario();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Agregar(int pacienteId, int medicoId, DateOnly fecha, TimeOnly hora, string motivo)
    {
        try
        {
            _service.Agregar(new Cita
            {
                PacienteId = pacienteId,
                MedicoId   = medicoId,
                Fecha      = fecha,
                Hora       = hora,
                Motivo     = motivo
            });

            TempData["Mensaje"] = "Cita agendada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is EntidadNoEncontradaException or OperacionInvalidaException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CargarListasParaFormulario();
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirmar(int id)
    {
        try
        {
            _service.ConfirmarCita(id);
            TempData["Mensaje"] = "Cita confirmada.";
        }
        catch (EntidadNoEncontradaException)
        {
            TempData["Error"] = "La cita no existe.";
        }

        return RedirectToAction(nameof(Index));
    }
}
