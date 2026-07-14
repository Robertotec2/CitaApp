using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces;

/// <summary>
/// Puerto de entrada (Input Port / Use Case) para las operaciones de Citas.
/// </summary>
public interface ICitaService
{
    List<Cita> ObtenerTodas();
    Cita ObtenerPorId(int id);
    List<Cita> ObtenerPorPaciente(int pacienteId);
    Cita Agregar(Cita cita);
    void ConfirmarCita(int id);
}
