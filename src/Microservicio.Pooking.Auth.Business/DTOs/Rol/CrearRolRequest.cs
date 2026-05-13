namespace Microservicio.Pooking.Auth.Business.DTOs.Rol;

/// <summary>
/// DTO de entrada para la creación de un nuevo rol en el catálogo.
/// </summary>
public class CrearRolRequest
{
    public string NombreRol { get; set; } = string.Empty;
    public string? DescripcionRol { get; set; }

    /// <summary>
    /// Identificador del usuario que ejecuta la operación (para auditoría).
    /// </summary>
    public string CreadoPorUsuario { get; set; } = string.Empty;
}