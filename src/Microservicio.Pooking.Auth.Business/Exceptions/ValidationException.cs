namespace Microservicio.Pooking.Auth.Business.Exceptions;

/// <summary>
/// Excepción lanzada cuando uno o más campos no cumplen las reglas de negocio.
/// Contiene la colección de errores de validación para devolver al cliente.
/// La API la traduce a HTTP 400 Bad Request.
/// </summary>
public class ValidationException : BusinessException
{
    /// <summary>
    /// Lista de mensajes de error de validación.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(string message, IReadOnlyCollection<string>? errors = null)
        : base(message)
    {
        Errors = errors ?? Array.Empty<string>();
    }
}