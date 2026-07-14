using CitasApp.Domain.Exceptions;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services;

/// <summary>
/// Caso de uso de Pacientes. No sabe si los datos vienen de Json, Csv o Sqlite —
/// solo conoce el puerto IPacienteRepository.
/// </summary>
public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _repository;

    public PacienteService(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public List<Paciente> ObtenerTodos() => _repository.ObtenerTodos();

    public Paciente ObtenerPorId(int id)
    {
        var paciente = _repository.ObtenerPorId(id);
        if (paciente is null)
            throw new EntidadNoEncontradaException("Paciente", id);

        return paciente;
    }
}
