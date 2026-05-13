using Microservicio.Pooking.Auth.Business.DTOs.Auth;

namespace Microservicio.Pooking.Auth.Business.Interfaces;

/// <summary>
/// Contrato del servicio de autenticación.
/// Valida credenciales, verifica estado del usuario y prepara
/// la información base para que la API genere el JWT.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Valida las credenciales del usuario y retorna la información
    /// necesaria para que la API construya el token JWT.
    /// Lanza UnauthorizedBusinessException si las credenciales son inválidas
    /// o el usuario está inactivo.
    /// </summary>
    Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}