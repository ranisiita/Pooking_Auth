namespace Microservicio.Pooking.Auth.DataManagement.Models;

/// <summary>
/// Modelo de datos desacoplado de EF Core que representa un usuario
/// dentro de la capa de Gestión de Datos.
/// Las capas superiores (negocio y API) trabajan con este modelo,
/// nunca con UsuarioAppEntity directamente.
/// </summary>
public class UsuarioDataModel
{
    // -------------------------------------------------------------------------
    // Identificación
    // -------------------------------------------------------------------------
    public int IdUsuario { get; set; }
    public Guid UsuarioGuid { get; set; }

    // -------------------------------------------------------------------------
    // Datos funcionales
    // -------------------------------------------------------------------------
    public string Username { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    // -------------------------------------------------------------------------
    // Estado y ciclo de vida
    // -------------------------------------------------------------------------
    public string EstadoUsuario { get; set; } = "ACT";
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
    // Roles asignados — proyección plana de nombres
    // -------------------------------------------------------------------------
    public IReadOnlyList<string> NombresRoles { get; set; } = [];
}