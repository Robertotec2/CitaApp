using CitasApp.Domain.Exceptions;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services;

/// <summary>
/// Caso de uso de Medicos.
/// </summary>
public class MedicoService
{
    private readonly IMedicoRepository _repository;

    public MedicoService(IMedicoRepository repository)
    {
        _repository = repository;
    }

    public List<Medico> ObtenerTodos() => _repository.ObtenerTodos();

    public Medico ObtenerPorId(int id)
    {
        var medico = _repository.ObtenerPorId(id);
        if (medico is null)
            throw new EntidadNoEncontradaException("Medico", id);

        return medico;
    }
}
