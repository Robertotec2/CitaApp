using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces;

/// <summary>
/// Puerto de salida (Output Port) para acceder y mutar los datos de citas.
/// Agregar y ConfirmarCita son parte del contrato para que los tres Adaptadores
/// (Json, Csv, Sqlite) ofrezcan exactamente las mismas capacidades de escritura.
/// </summary>
public interface ICitaRepository
{
    List<Cita> ObtenerTodos();
    Cita?      ObtenerPorId(int id);
    List<Cita> ObtenerPorPaciente(int pacienteId);
    List<Cita> ObtenerPorMedico(int medicoId);
    Cita       Agregar(Cita cita);
    bool       ConfirmarCita(int id);
    bool       Eliminar(int id);
}
