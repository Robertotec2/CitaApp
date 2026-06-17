using System.Net;
using System.Text.Json;
using CitasApp.Domain.Exceptions;
using CitasApp.Infrastructure.Exceptions;

namespace CitasApp.Api.Middleware;

/// <summary>
/// Intercepta cualquier excepcion no controlada que suba desde Application o
/// Infrastructure y la convierte en una respuesta JSON consistente con el
/// codigo HTTP correcto. Asi los controladores quedan limpios — no necesitan
/// try/catch repetido en cada accion.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntidadNoEncontradaException ex)
        {
            await EscribirRespuesta(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (OperacionInvalidaException ex)
        {
            await EscribirRespuesta(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (PersistenciaException ex)
        {
            _logger.LogError(ex, "Fallo de persistencia.");
            await EscribirRespuesta(context, HttpStatusCode.InternalServerError,
                "Ocurrio un problema al acceder a los datos. Intenta de nuevo mas tarde.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado.");
            await EscribirRespuesta(context, HttpStatusCode.InternalServerError,
                "Ocurrio un error inesperado en el servidor.");
        }
    }

    private static async Task EscribirRespuesta(HttpContext context, HttpStatusCode statusCode, string mensaje)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            error = mensaje,
            status = (int)statusCode
        });

        await context.Response.WriteAsync(payload);
    }
}
