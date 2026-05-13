namespace Microservicio.Pooking.Auth.DataManagement.Models;

/// <summary>
/// Modelo de datos desacoplado de EF Core que representa un rol
/// dentro de la capa de Gestión de Datos.
/// </summary>
public class RolDataModel
{
    // -------------------------------------------------------------------------
    // Identificación
    // -------------------------------------------------------------------------
    public int IdRol { get; set; }
    public Guid RolGuid { get; set; }

    // -------------------------------------------------------------------------
    // Datos funcionales
    // -------------------------------------------------------------------------
    public string NombreRol { get; set; } = string.Empty;
    public string? DescripcionRol { get; set; }

    // -------------------------------------------------------------------------
    // Estado y ciclo de vida
    // -------------------------------------------------------------------------
    public string EstadoRol { get; set; } = "ACT";
    public bool EsEliminado { get; set; } = false;
    public bool Activo { get; set; } = true;

    // -------------------------------------------------------------------------
    // Auditoría
    // -------------------------------------------------------------------------
    public DateTime FechaRegistroUtc { get; set; }
    public string CreadoPorUsuario { get; set; } = string.Empty;
    public string? ModificadoPorUsuario { get; set; }
    public DateTime? FechaModificacionUtc { get; set; }
}