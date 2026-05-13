namespace Microservicio.Pooking.Auth.Business.DTOs.Auth;

/// <summary>
/// DTO de salida del proceso de autenticación.
/// Business popula: UsuarioGuid, Username, Correo, Activo, Roles.
/// La capa API (AuthController) popula: Token y ExpirationUtc
/// una vez que genera el JWT con JwtSettings.
/// </summary>
public class LoginResponse
{
    public Guid UsuarioGuid { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public bool Activo { get; set; }

    /// <summary>
    /// Roles activos del usuario. La API los incluirá como claims en el JWT.
    /// </summary>
    public IReadOnlyList<string> Roles { get; set; } = [];

    /// <summary>
    /// Token JWT generado por AuthController. Business no lo popula.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Fecha de expiración del token. La setea AuthController junto con el JWT.
    /// </summary>
    public DateTime? ExpirationUtc { get; set; }
}