namespace Microservicio.Pooking.Auth.Business.DTOs.Usuario;

/// <summary>
/// DTO de salida que representa un usuario en las respuestas de la API.
/// Solo expone campos seguros — nunca hash ni salt.
/// </summary>
public class UsuarioResponse
{
    public Guid UsuarioGuid { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string EstadoUsuario { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaRegistroUtc { get; set; }

    /// <summary>
    /// Roles activos asignados al usuario.
    /// </summary>
    public IReadOnlyList<string> Roles { get; set; } = [];
}