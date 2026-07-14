using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces;

/// <summary>
/// Puerto de entrada (Input Port / Use Case) para las operaciones de Pacientes.
/// </summary>
public interface IPacienteService
{
    List<Paciente> ObtenerTodos();
    Paciente ObtenerPorId(int id);
}
