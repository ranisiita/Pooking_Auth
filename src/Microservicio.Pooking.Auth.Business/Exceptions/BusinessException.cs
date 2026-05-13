namespace Microservicio.Pooking.Auth.Business.Exceptions;

/// <summary>
/// Error de negocio genérico, sin acoplar a códigos HTTP.
/// </summary>
public class BusinessException : Exception
{
    public string? Codigo { get; }

    public BusinessException(string mensaje, string? codigo = null, Exception? interna = null)
        : base(mensaje, interna)
    {
        Codigo = codigo;
    }
}
