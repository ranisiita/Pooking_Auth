namespace Microservicio.Pooking.Auth.DataAccess.Entities;

/// <summary>
/// Entidad que representa la tabla puente booking.usuarios_roles.
/// Materializa la relación N:M entre UsuarioApp y Rol.
/// </summary>
public class UsuarioRolEntity
{
    // -------------------------------------------------------------------------
    // [1] Identificación técnica
    // -------------------------------------------------------------------------

    /// <summary>
    /// PK surrogate de la tabla puente.
    /// </summary>
    public int IdUsuarioRol { get; set; }

    // -------------------------------------------------------------------------
    // [2] Claves foráneas
    // -------------------------------------------------------------------------

    public int IdUsuario { get; set; }
    public int IdRol { get; set; }

    // -------------------------------------------------------------------------
    // [3] Estado y ciclo de vida
    // -------------------------------------------------------------------------

    /// <summary>
    /// ACT = Activo | INA = Inactivo.
    /// </summary>
    public string EstadoUsuarioRol { get; set; } = "ACT";

    /// <summary>
    /// Borrado lógico. 0 = vigente, 1 = eliminado.
    /// </summary>
    public bool EsEliminado { get; set; } = false;

    /// <summary>
    /// Flag operativo rápido complementario al estado.
    /// </summary>
    public bool Activo { get; set; } = true;

    // -------------------------------------------------------------------------
    // [4] Auditoría
    // -------------------------------------------------------------------------

    public DateTime FechaRegistroUtc { get; set; }
    public string CreadoPorUsuario { get; set; } = string.Empty;
    public string? ModificadoPorUsuario { get; set; }
    public DateTime? FechaModificacionUtc { get; set; }

    // -------------------------------------------------------------------------
    // Navegación
    // -------------------------------------------------------------------------

    /// <summary>
    /// Usuario al que pertenece esta asignación.
    /// </summary>
    public UsuarioAppEntity Usuario { get; set; } = null!;

    /// <summary>
    /// Rol asignado en esta relación.
    /// </summary>
    public RolEntity Rol { get; set; } = null!;
}