using Microservicio.Pooking.Auth.Business.Exceptions;
using Microservicio.Pooking.Auth.Business.DTOs.Rol;

namespace Microservicio.Pooking.Auth.Business.Validators;

/// <summary>
/// Validador de reglas de negocio para operaciones sobre roles.
/// </summary>
public static class RolValidator
{
    public static void ValidarCrear(CrearRolRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.NombreRol))
            errores.Add("El nombre del rol es obligatorio.");
        else if (request.NombreRol.Length > 50)
            errores.Add("El nombre del rol no puede superar 50 caracteres.");

        if (request.DescripcionRol is not null && request.DescripcionRol.Length > 200)
            errores.Add("La descripción no puede superar 200 caracteres.");

        if (string.IsNullOrWhiteSpace(request.CreadoPorUsuario))
            errores.Add("El campo CreadoPorUsuario es obligatorio para auditoría.");

        if (errores.Count > 0)
            throw new ValidationException("La solicitud de creación de rol contiene errores.", errores);
    }

    public static void ValidarAsignacion(AsignarRolRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.UsuarioGuid))
            errores.Add("El GUID del usuario es obligatorio.");
        else if (!Guid.TryParse(request.UsuarioGuid, out _))
            errores.Add($"No se encontró el usuario."); // Specific message the user might expect

        if (string.IsNullOrWhiteSpace(request.RolGuid))
            errores.Add("El GUID del rol es obligatorio.");
        else if (!Guid.TryParse(request.RolGuid, out _))
            errores.Add($"No se encontró el rol.");

        if (string.IsNullOrWhiteSpace(request.EjecutadoPorUsuario))
            errores.Add("El campo EjecutadoPorUsuario es obligatorio para auditoría.");

        if (errores.Count > 0)
            throw new ValidationException("La solicitud de asignación de rol contiene errores.", errores);
    }
}