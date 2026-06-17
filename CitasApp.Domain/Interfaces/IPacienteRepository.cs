using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces;

/// <summary>
/// Puerto de salida (Output Port) para acceder a los datos de pacientes.
/// Cualquier Adaptador (JsonPacienteRepository, CsvPacienteRepository,
/// SqlitePacienteRepository...) debe cumplir este contrato.
/// </summary>
public interface IPacienteRepository
{
    List<Paciente>  ObtenerTodos();
    Paciente?       ObtenerPorId(int id);
}
