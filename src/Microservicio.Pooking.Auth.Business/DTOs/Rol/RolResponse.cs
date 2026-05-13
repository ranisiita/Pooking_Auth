namespace Microservicio.Pooking.Auth.Business.DTOs.Rol;

/// <summary>
/// DTO de salida que representa un rol en las respuestas de la API.
/// </summary>
public class RolResponse
{
    public Guid RolGuid { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public string? DescripcionRol { get; set; }
    public string EstadoRol { get; set; } = string.Empty;
    public bool Activo { get; set; }
}