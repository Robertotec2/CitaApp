using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces;

/// <summary>
/// Puerto de salida (Output Port) para acceder a los datos de medicos.
/// </summary>
public interface IMedicoRepository
{
    List<Medico>  ObtenerTodos();
    Medico?       ObtenerPorId(int id);
    Medico        Agregar(Medico medico);
}
