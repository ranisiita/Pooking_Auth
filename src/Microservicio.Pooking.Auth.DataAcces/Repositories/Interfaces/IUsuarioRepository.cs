using Microservicio.Pooking.Auth.DataAccess.Common;
using Microservicio.Pooking.Auth.DataAccess.Entities;

namespace Microservicio.Pooking.Auth.DataAccess.Repositories.Interfaces;

/// <summary>
/// Contrato de acceso a datos para la entidad UsuarioApp (tabla booking.usuario_app).
/// Define las operaciones de escritura y lectura directa del repositorio principal.
/// Las consultas proyectadas y paginadas complejas viven en UsuarioQueryRepository.
/// </summary>
public interface IUsuarioRepository
{
    // -------------------------------------------------------------------------
    // Lecturas simples — búsqueda por identificador
    // -------------------------------------------------------------------------

    /// <summary>
    /// Obtiene un usuario por su PK interna.
    /// Retorna null si no existe o si está eliminado lógicamente.
    /// </summary>
    Task<UsuarioAppEntity?> ObtenerPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un usuario por su GUID público (expuesto en la API).
    /// Retorna null si no existe o si está eliminado lógicamente.
    /// </summary>
    Task<UsuarioAppEntity?> ObtenerPorGuidAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un usuario por su username exacto.
    /// Retorna null si no existe o si está eliminado lógicamente.
    /// </summary>
    Task<UsuarioAppEntity?> ObtenerPorUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un usuario por su correo electrónico.
    /// Retorna null si no existe o si está eliminado lógicamente.
    /// </summary>
    Task<UsuarioAppEntity?> ObtenerPorCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default);

    Task<UsuarioAppEntity?> ObtenerParaActualizarAsync(
    int idUsuario,
    CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Lecturas con roles — Include de navegación
    // -------------------------------------------------------------------------

    /// <summary>
    /// Obtiene un usuario con sus roles activos cargados (eager loading).
    /// Útil para validaciones de autorización en la capa de negocio.
    /// </summary>
    Task<UsuarioAppEntity?> ObtenerPorGuidConRolesAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Lecturas paginadas
    // -------------------------------------------------------------------------

    /// <summary>
    /// Retorna una página de usuarios vigentes (es_eliminado = 0).
    /// </summary>
    /// <param name="paginaActual">Número de página base 1.</param>
    /// <param name="tamanoPagina">Registros por página.</param>
    /// <param name="cancellationToken"></param>
    Task<PagedResult<UsuarioAppEntity>> ObtenerTodosPaginadoAsync(
        int paginaActual,
        int tamanoPagina,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Verificaciones — evitan roundtrips innecesarios cargando entidades completas
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifica si ya existe un usuario con ese username (sin importar estado).
    /// Se usa antes de crear o actualizar para garantizar unicidad.
    /// </summary>
    Task<bool> ExisteUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si ya existe un usuario con ese correo (sin importar estado).
    /// </summary>
    Task<bool> ExisteCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida si el username o correo ya están registrados, y a su vez si el rol especificado existe.
    /// Ejecuta todo en una sola consulta para evitar problemas de concurrencia en la base de datos.
    /// </summary>
    Task<(bool ExisteUsername, bool ExisteCorreo, bool ExisteRol, int IdRol)> ValidarRegistroAsync(
        string username,
        string correo,
        string nombreRol,
        CancellationToken cancellationToken = default);

    /// <summary>
    // Obtiene un token mediante username o correo, incluyendo sus roles activos.
    /// </summary>
    Task<UsuarioAppEntity?> ObtenerPorUsernameOCorreoConRolesAsync(
        string identificador,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Escritura
    // -------------------------------------------------------------------------

    /// <summary>
    /// Persiste un nuevo usuario en la base de datos.
    /// El IdUsuario y UsuarioGuid son asignados por la BD; no deben setearse antes.
    /// </summary>
    Task AgregarAsync(
        UsuarioAppEntity usuario,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca la entidad como modificada en el ChangeTracker de EF Core.
    /// El llamador es responsable de invocar SaveChangesAsync en la unidad de trabajo.
    /// </summary>
    void Actualizar(UsuarioAppEntity usuario);

    /// <summary>
    /// Realiza borrado lógico: setea es_eliminado = 1 y estado = 'INA'.
    /// No elimina físicamente el registro de la base de datos.
    /// </summary>
    void EliminarLogico(UsuarioAppEntity usuario);
}