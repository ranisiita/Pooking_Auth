using Microservicio.Pooking.Auth.Business.DTOs.Usuario;
using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.Business.Interfaces;

/// <summary>
/// Contrato del servicio de negocio para la gestión de usuarios.
/// Define los casos de uso que expone la capa de negocio a la API.
/// La API depende de esta interfaz, nunca de la implementación directa.
/// </summary>
public interface IUsuarioService
{
    // -------------------------------------------------------------------------
    // Consultas
    // -------------------------------------------------------------------------

    Task<UsuarioResponse?> ObtenerPorGuidAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default);

    Task<UsuarioResponse?> ObtenerPorGuidConRolesAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default);

    Task<DataPagedResult<UsuarioResponse>> BuscarAsync(
        UsuarioFiltroRequest filtro,
        CancellationToken cancellationToken = default);

    Task<bool> UsernameDisponibleAsync(string username, CancellationToken ct = default);
    Task<bool> CorreoDisponibleAsync(string correo, CancellationToken ct = default);
    /// <summary>
    /// Retorna el ID interno del usuario a partir de su GUID público.
    /// Solo para uso interno entre servicios del backend — nunca exponer en la API.
    /// </summary>
    Task<int> ObtenerIdInternoAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // Comandos
    // -------------------------------------------------------------------------

    /// <summary>
    /// Registra un nuevo usuario, genera el hash de contraseña
    /// y le asigna el rol indicado automáticamente.
    /// </summary>
    Task<UsuarioResponse> CrearAsync(
        CrearUsuarioRequest request,
        CancellationToken cancellationToken = default);

    Task<UsuarioResponse> ActualizarAsync(
        ActualizarUsuarioRequest request,
        CancellationToken cancellationToken = default);

    Task EliminarLogicoAsync(
        Guid usuarioGuid,
        string modificadoPorUsuario,
        CancellationToken cancellationToken = default);
}