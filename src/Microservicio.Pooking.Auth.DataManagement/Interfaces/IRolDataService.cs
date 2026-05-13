using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.DataManagement.Interfaces;

/// <summary>
/// Contrato del servicio de datos para el dominio de roles y asignaciones.
/// Cubre el catálogo de roles y la relación usuario-rol.
/// </summary>
public interface IRolDataService
{
    // -------------------------------------------------------------------------
    // Lecturas — Rol
    // -------------------------------------------------------------------------

    Task<RolDataModel?> ObtenerRolPorGuidAsync(
        Guid rolGuid,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RolDataModel>> ObtenerTodosLosRolesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna las asignaciones completas usuario-rol, incluyendo estado,
    /// fecha de creación y quién las creó. Útil para el detalle de permisos.
    /// </summary>
    Task<IReadOnlyList<UsuarioRolDataModel>> ObtenerAsignacionesDeUsuarioAsync(
        int idUsuario,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Verificaciones — Rol
    // -------------------------------------------------------------------------

    Task<bool> ExisteNombreRolAsync(
        string nombreRol,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Escritura — Rol
    // -------------------------------------------------------------------------

    Task<RolDataModel> CrearRolAsync(
        RolDataModel model,
        CancellationToken cancellationToken = default);

    Task<bool> EliminarLogicoRolAsync(
        int idRol,
        string modificadoPorUsuario,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Lecturas — Asignaciones usuario-rol
    // -------------------------------------------------------------------------

    Task<IReadOnlyList<RolDataModel>> ObtenerRolesDeUsuarioAsync(
        int idUsuario,
        CancellationToken cancellationToken = default);

    Task<bool> UsuarioTieneRolAsync(
        int idUsuario,
        int idRol,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Escritura — Asignaciones usuario-rol
    // -------------------------------------------------------------------------

    Task AsignarRolAsync(
        int idUsuario,
        int idRol,
        string creadoPorUsuario,
        CancellationToken cancellationToken = default);

    Task<bool> RevocarRolAsync(
        int idUsuario,
        int idRol,
        string modificadoPorUsuario,
        CancellationToken cancellationToken = default);
}