namespace CitasApp.Api.DTOs;

/// <summary>
/// Forma de entrada para crear una cita desde la API. Se separa del modelo de
/// Dominio para no exponer detalles internos en el contrato HTTP (Id y Estado
/// los decide el servidor, no el cliente).
/// </summary>
public class CrearCitaDto
{
    public int      PacienteId { get; set; }
    public int      MedicoId   { get; set; }
    public DateOnly Fecha      { get; set; }
    public TimeOnly Hora       { get; set; }
    public string   Motivo     { get; set; } = string.Empty;
}
