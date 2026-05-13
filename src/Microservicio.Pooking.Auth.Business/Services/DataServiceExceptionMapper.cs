using Microservicio.Pooking.Auth.Business.Exceptions;

namespace Microservicio.Pooking.Auth.Business.Services;

/// <summary>
/// Traduce excepciones de la capa de datos (mensajes conocidos) a excepciones de negocio.
/// </summary>
internal static class DataServiceExceptionMapper
{
    public static void PropagarSiInvalidOperation(InvalidOperationException ex)
    {
        if (ex.Message.Contains("No se encontró", StringComparison.OrdinalIgnoreCase))
            throw new NotFoundException(ex.Message);

        throw new ValidationException(ex.Message);
    }
}
