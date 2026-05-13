using Microservicio.Pooking.Auth.DataAccess.Common;
using Microservicio.Pooking.Auth.DataAccess.Entities;

namespace Microservicio.Pooking.Auth.DataAccess.Repositories.Interfaces;

/// <summary>
/// Contrato de acceso a datos para las entidades Rol y UsuarioRol.
/// Cubre el catálogo de roles (booking.rol) y la asignación de roles
/// a usuarios (booking.usuarios_roles).
/// Ambas tablas se gestionan desde aquí porque usuarios_roles no tiene
/// identidad de negocio propia fuera de la relación usuario-rol.
/// </summary>
public interface IRolRepository
{
    // -------------------------------------------------------------------------
    // Lecturas — Rol
    // -------------------------------------------------------------------------

    /// <summary>
    /// Obtiene un rol por su PK interna.
    /// Retorna null si no existe o está eliminado lógicamente.
    /// </summary>
    Task<RolEntity?> ObtenerRolPorIdAsync(
        int idRol,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un rol por su GUID público.
    /// Retorna null si no existe o está eliminado lógicamente.
    /// </summary>
    Task<RolEntity?> ObtenerRolPorGuidAsync(
        Guid rolGuid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un rol por su nombre exacto (ej. "ADMINISTRADOR").
    /// Retorna null si no existe.
    /// </summary>
    Task<RolEntity?> ObtenerRolPorNombreAsync(
        string nombreRol,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna todos los roles activos y vigentes del catálogo.
    /// </summary>
    Task<IReadOnlyList<RolEntity>> ObtenerTodosLosRolesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna una página de roles vigentes (es_eliminado = 0).
    /// </summary>
    Task<PagedResult<RolEntity>> ObtenerRolesPaginadoAsync(
        int paginaActual,
        int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task<RolEntity?> ObtenerRolParaActualizarAsync(
    int idRol,
    CancellationToken cancellationToken = default);

    Task<UsuarioRolEntity?> ObtenerAsignacionParaActualizarAsync(
        int idUsuario,
        int idRol,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Verificaciones — Rol
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifica si ya existe un rol con ese nombre (sin importar estado).
    /// Se usa antes de crear para garantizar unicidad.
    /// </summary>
    Task<bool> ExisteNombreRolAsync(
        string nombreRol,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Escritura — Rol
    // -------------------------------------------------------------------------

    /// <summary>
    /// Persiste un nuevo rol en el catálogo.
    /// </summary>
    Task AgregarRolAsync(
        RolEntity rol,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca el rol como modificado en el ChangeTracker de EF Core.
    /// </summary>
    void ActualizarRol(RolEntity rol);

    /// <summary>
    /// Borrado lógico del rol: setea es_eliminado = 1 y estado = 'INA'.
    /// </summary>
    void EliminarLogicoRol(RolEntity rol);

    // -------------------------------------------------------------------------
    // Lecturas — UsuarioRol (asignaciones)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Retorna todos los roles activos asignados a un usuario específico.
    /// Incluye la navegación a RolEntity para acceder al nombre del rol.
    /// </summary>
    Task<IReadOnlyList<UsuarioRolEntity>> ObtenerRolesDeUsuarioAsync(
        int idUsuario,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna la asignación específica usuario-rol si existe y está activa.
    /// Retorna null si la asignación no existe o está eliminada lógicamente.
    /// </summary>
    Task<UsuarioRolEntity?> ObtenerAsignacionAsync(
        int idUsuario,
        int idRol,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Verificaciones — UsuarioRol
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifica si un usuario ya tiene asignado un rol específico y está activo.
    /// Se usa antes de asignar para evitar duplicados.
    /// </summary>
    Task<bool> UsuarioTieneRolAsync(
        int idUsuario,
        int idRol,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Escritura — UsuarioRol (asignaciones)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Persiste una nueva asignación usuario-rol.
    /// La capa de negocio debe verificar previamente que no exista ya con
    /// UsuarioTieneRolAsync para evitar violación de la restricción única.
    /// </summary>
    Task AgregarAsignacionAsync(
        UsuarioRolEntity usuarioRol,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Borrado lógico de la asignación: setea es_eliminado = 1 y estado = 'INA'.
    /// No elimina el registro físico para mantener trazabilidad histórica.
    /// </summary>
    void EliminarLogicoAsignacion(UsuarioRolEntity usuarioRol);
}