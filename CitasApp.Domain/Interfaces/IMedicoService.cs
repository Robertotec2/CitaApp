using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces;

/// <summary>
/// Puerto de entrada (Input Port / Use Case) para las operaciones de Medicos.
/// </summary>
public interface IMedicoService
{
    List<Medico> ObtenerTodos();
    Medico ObtenerPorId(int id);
}
