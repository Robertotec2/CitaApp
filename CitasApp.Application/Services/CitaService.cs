using CitasApp.Domain.Exceptions;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services;

/// <summary>
/// Caso de uso de Citas. Centraliza las reglas de negocio: una cita no puede
/// crearse para un paciente o medico que no existe, sin motivo, ni en el pasado.
/// </summary>
public class CitaService
{
    private readonly ICitaRepository     _citaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMedicoRepository   _medicoRepository;

    public CitaService(ICitaRepository citaRepository,
                        IPacienteRepository pacienteRepository,
                        IMedicoRepository medicoRepository)
    {
        _citaRepository     = citaRepository;
        _pacienteRepository = pacienteRepository;
        _medicoRepository   = medicoRepository;
    }

    public List<Cita> ObtenerTodas() => _citaRepository.ObtenerTodos();

    public Cita ObtenerPorId(int id)
    {
        var cita = _citaRepository.ObtenerPorId(id);
        if (cita is null)
            throw new EntidadNoEncontradaException("Cita", id);

        return cita;
    }

    public List<Cita> ObtenerPorPaciente(int pacienteId)
    {
        // El paciente debe existir; una lista de citas vacia SI es un resultado valido.
        if (_pacienteRepository.ObtenerPorId(pacienteId) is null)
            throw new EntidadNoEncontradaException("Paciente", pacienteId);

        return _citaRepository.ObtenerPorPaciente(pacienteId);
    }

    public Cita Agregar(Cita cita)
    {
        if (_pacienteRepository.ObtenerPorId(cita.PacienteId) is null)
            throw new EntidadNoEncontradaException("Paciente", cita.PacienteId);

        if (_medicoRepository.ObtenerPorId(cita.MedicoId) is null)
            throw new EntidadNoEncontradaException("Medico", cita.MedicoId);

        if (string.IsNullOrWhiteSpace(cita.Motivo))
            throw new OperacionInvalidaException("El motivo de la cita es obligatorio.");

        if (cita.Fecha == default)
            throw new OperacionInvalidaException("La fecha de la cita es obligatoria.");

        if (cita.Fecha < DateOnly.FromDateTime(DateTime.Today))
            throw new OperacionInvalidaException("No se pueden agendar citas en una fecha pasada.");

        cita.Estado = "Pendiente";
        return _citaRepository.Agregar(cita);
    }

    public void ConfirmarCita(int id)
    {
        var confirmada = _citaRepository.ConfirmarCita(id);
        if (!confirmada)
            throw new EntidadNoEncontradaException("Cita", id);
    }
}
