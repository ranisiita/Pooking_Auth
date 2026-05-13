namespace Microservicio.Pooking.Auth.DataAccess.Entities;

/// <summary>
/// Entidad que representa la tabla booking.rol.
/// Catálogo de roles disponibles en el sistema (ej. ADMINISTRADOR, VENDEDOR).
/// </summary>
public class RolEntity
{
    // -------------------------------------------------------------------------
    // [1] Identificación técnica
    // -------------------------------------------------------------------------

    /// <summary>
    /// PK interna. No se expone en la API.
    /// </summary>
    public int IdRol { get; set; }

    /// <summary>
    /// Identificador público expuesto en la API REST.
    /// </summary>
    public Guid RolGuid { get; set; }

    // -------------------------------------------------------------------------
    // [2] Datos funcionales
    // -------------------------------------------------------------------------

    public string NombreRol { get; set; } = string.Empty;
    public string? DescripcionRol { get; set; }

    // -------------------------------------------------------------------------
    // [3] Estado y ciclo de vida
    // -------------------------------------------------------------------------

    /// <summary>
    /// ACT = Activo | INA = Inactivo.
    /// </summary>
    public string EstadoRol { get; set; } = "ACT";

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
    /// Usuarios que tienen asignado este rol (vía tabla puente usuarios_roles).
    /// </summary>
    public ICollection<UsuarioRolEntity> UsuariosRoles { get; set; } = [];
}