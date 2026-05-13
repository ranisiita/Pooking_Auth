namespace Microservicio.Pooking.Auth.Business.DTOs.Usuario;

/// <summary>
/// DTO de entrada para actualizar los datos de un usuario existente.
/// El GUID identifica el registro; los demás campos son los actualizables.
/// </summary>
public class ActualizarUsuarioRequest
{
    /// <summary>
    /// GUID público del usuario a actualizar.
    /// </summary>
    public Guid UsuarioGuid { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del usuario que ejecuta la operación (para auditoría).
    /// </summary>
    public string ModificadoPorUsuario { get; set; } = string.Empty;
}