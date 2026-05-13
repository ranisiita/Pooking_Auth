using Microservicio.Pooking.Auth.Business.DTOs.Rol;

namespace Microservicio.Pooking.Auth.Business.Interfaces;

/// <summary>
/// Contrato del servicio de negocio para la gestión de roles
/// y asignaciones usuario-rol.
/// </summary>
public interface IRolService
{
    // -------------------------------------------------------------------------
    // Catálogo de roles
    // -------------------------------------------------------------------------

    Task<IReadOnlyList<RolResponse>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default);

    Task<RolResponse?> ObtenerPorGuidAsync(
        Guid rolGuid,
        CancellationToken cancellationToken = default);

    Task<RolResponse> CrearAsync(
        CrearRolRequest request,
        CancellationToken cancellationToken = default);

    Task EliminarLogicoAsync(
        Guid rolGuid,
        string modificadoPorUsuario,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Asignaciones usuario-rol
    // -------------------------------------------------------------------------

    Task<IReadOnlyList<RolResponse>> ObtenerRolesDeUsuarioAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna el detalle completo de las asignaciones de un usuario,
    /// incluyendo estado, fecha de asignación y quién la creó.
    /// </summary>
    Task<IReadOnlyList<UsuarioRolResponse>> ObtenerAsignacionesDeUsuarioAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default);

    Task AsignarRolAsync(
        AsignarRolRequest request,
        CancellationToken cancellationToken = default);

    Task RevocarRolAsync(
        AsignarRolRequest request,
        CancellationToken cancellationToken = default);
}