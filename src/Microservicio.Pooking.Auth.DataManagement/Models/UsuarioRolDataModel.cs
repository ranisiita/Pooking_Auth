namespace Microservicio.Pooking.Auth.DataManagement.Models;

/// <summary>
/// Modelo de datos desacoplado de EF Core que representa una asignación
/// usuario-rol (tabla booking.usuarios_roles).
/// Permite que las capas superiores trabajen con la asignación
/// sin depender de UsuarioRolEntity directamente.
/// </summary>
public class UsuarioRolDataModel
{
    // -------------------------------------------------------------------------
    // Identificación
    // -------------------------------------------------------------------------
    public int IdUsuarioRol { get; set; }

    // -------------------------------------------------------------------------
    // Claves de relación
    // -------------------------------------------------------------------------
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }

    // -------------------------------------------------------------------------
    // Estado y ciclo de vida
    // -------------------------------------------------------------------------
    public string EstadoUsuarioRol { get; set; } = "ACT";
    public bool EsEliminado { get; set; } = false;
    public bool Activo { get; set; } = true;

    // -------------------------------------------------------------------------
    // Auditoría
    // -------------------------------------------------------------------------
    public DateTime FechaRegistroUtc { get; set; }
    public string CreadoPorUsuario { get; set; } = string.Empty;
    public string? ModificadoPorUsuario { get; set; }
    public DateTime? FechaModificacionUtc { get; set; }

    // -------------------------------------------------------------------------
    // Datos navegados — proyección plana para evitar objetos anidados
    // -------------------------------------------------------------------------

    /// <summary>
    /// Nombre del rol de esta asignación.
    /// Se popula cuando se necesita mostrar la asignación con contexto.
    /// </summary>
    public string NombreRol { get; set; } = string.Empty;

    /// <summary>
    /// Username del usuario de esta asignación.
    /// Se popula cuando se necesita mostrar la asignación con contexto.
    /// </summary>
    public string Username { get; set; } = string.Empty;
}