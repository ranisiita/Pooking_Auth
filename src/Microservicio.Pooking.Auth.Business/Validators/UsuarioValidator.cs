using Microservicio.Pooking.Auth.Business.Exceptions;
using Microservicio.Pooking.Auth.Business.DTOs.Usuario;

namespace Microservicio.Pooking.Auth.Business.Validators;

/// <summary>
/// Validador de reglas de negocio para operaciones sobre usuarios.
/// No usa DataAnnotations — aplica lógica real de dominio.
/// Lanza ValidationException con todos los errores encontrados.
/// </summary>
public static class UsuarioValidator
{
    // -------------------------------------------------------------------------
    // Crear usuario
    // -------------------------------------------------------------------------

    public static void ValidarCrear(CrearUsuarioRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username))
            errores.Add("El username es obligatorio.");
        else if (request.Username.Length < 4)
            errores.Add("El username debe tener al menos 4 caracteres.");
        else if (request.Username.Length > 50)
            errores.Add("El username no puede superar 50 caracteres.");

        if (string.IsNullOrWhiteSpace(request.Correo))
            errores.Add("El correo es obligatorio.");
        else if (!EsCorreoValido(request.Correo))
            errores.Add("El formato del correo electrónico no es válido.");
        else if (request.Correo.Length > 120)
            errores.Add("El correo no puede superar 120 caracteres.");

        if (string.IsNullOrWhiteSpace(request.Password))
            errores.Add("La contraseña es obligatoria.");
        else if (request.Password.Length < 8)
            errores.Add("La contraseña debe tener al menos 8 caracteres.");

        if (string.IsNullOrWhiteSpace(request.NombreRol))
            errores.Add("El rol es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.CreadoPorUsuario))
            errores.Add("El campo CreadoPorUsuario es obligatorio para auditoría.");

        if (errores.Count > 0)
            throw new ValidationException("La solicitud de creación contiene errores.", errores);
    }

    // -------------------------------------------------------------------------
    // Actualizar usuario
    // -------------------------------------------------------------------------

    public static void ValidarActualizar(ActualizarUsuarioRequest request)
    {
        var errores = new List<string>();

        if (request.UsuarioGuid == Guid.Empty)
            errores.Add("El GUID del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Username))
            errores.Add("El username es obligatorio.");
        else if (request.Username.Length < 4)
            errores.Add("El username debe tener al menos 4 caracteres.");
        else if (request.Username.Length > 50)
            errores.Add("El username no puede superar 50 caracteres.");

        if (string.IsNullOrWhiteSpace(request.Correo))
            errores.Add("El correo es obligatorio.");
        else if (!EsCorreoValido(request.Correo))
            errores.Add("El formato del correo electrónico no es válido.");

        if (string.IsNullOrWhiteSpace(request.ModificadoPorUsuario))
            errores.Add("El campo ModificadoPorUsuario es obligatorio para auditoría.");

        if (errores.Count > 0)
            throw new ValidationException("La solicitud de actualización contiene errores.", errores);
    }

    // -------------------------------------------------------------------------
    // Login
    // -------------------------------------------------------------------------

    public static void ValidarLogin(string identificador, string password)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(identificador))
            errores.Add("El usuario o correo electrónico es obligatorio.");

        if (string.IsNullOrWhiteSpace(password))
            errores.Add("La contraseña es obligatoria.");

        if (errores.Count > 0)
            throw new ValidationException("Las credenciales son inválidas.", errores);
    }

    // -------------------------------------------------------------------------
    // Helpers privados
    // -------------------------------------------------------------------------

    private static bool EsCorreoValido(string correo)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(correo);
            return addr.Address == correo;
        }
        catch
        {
            return false;
        }
    }
}