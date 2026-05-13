namespace Microservicio.Pooking.Auth.Business.Exceptions;

/// <summary>
/// Recurso solicitado no existe o no está disponible para la operación.
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string mensaje, string? codigo = null)
        : base(mensaje, codigo)
    {
    }
}
