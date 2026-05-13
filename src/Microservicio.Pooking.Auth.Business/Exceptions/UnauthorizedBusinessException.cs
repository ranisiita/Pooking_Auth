namespace Microservicio.Pooking.Auth.Business.Exceptions;

/// <summary>
/// Excepción lanzada cuando un usuario no tiene autorización para ejecutar
/// una operación, o cuando las credenciales son inválidas.
/// La API la traduce a HTTP 401 Unauthorized.
/// </summary>
public class UnauthorizedBusinessException : BusinessException
{
    public UnauthorizedBusinessException(string message) : base(message)
    {
    }
}