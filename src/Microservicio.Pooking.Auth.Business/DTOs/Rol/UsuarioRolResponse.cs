namespace Microservicio.Pooking.Auth.Business.DTOs.Rol;

/// <summary>
/// DTO de salida que representa una asignación usuario-rol en las respuestas de la API.
/// Expone los datos de la relación de forma plana sin objetos anidados.
/// Nunca expone PKs internas — la API identifica asignaciones por UsuarioGuid + RolGuid.
/// </summary>
public class UsuarioRolResponse
{
    public string Username { get; set; } = string.Empty;
    public string NombreRol { get; set; } = string.Empty;
    public string EstadoUsuarioRol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaRegistroUtc { get; set; }
    public string CreadoPorUsuario { get; set; } = string.Empty;
}