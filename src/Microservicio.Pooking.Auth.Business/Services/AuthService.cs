using Microservicio.Pooking.Auth.Business.DTOs.Auth;
using Microservicio.Pooking.Auth.Business.Exceptions;
using Microservicio.Pooking.Auth.Business.Interfaces;
using Microservicio.Pooking.Auth.Business.Mappers;
using Microservicio.Pooking.Auth.Business.Validators;
using Microservicio.Pooking.Auth.DataManagement.Interfaces;

namespace Microservicio.Pooking.Auth.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioDataService _usuarioDataService;

    public AuthService(IUsuarioDataService usuarioDataService)
    {
        _usuarioDataService = usuarioDataService;
    }

    public async Task<LoginResponse> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken = default)
    {
        UsuarioValidator.ValidarLogin(request.Identificador, request.Password);

        // 1. Buscar usuario y credenciales en un solo query para evitar
        // race conditions de Npgsql con poolers externos (como Supavisor).
        var resultadoDb = await _usuarioDataService
            .ObtenerParaAutenticacionAsync(request.Identificador, cancellationToken);

        if (resultadoDb is null)
            throw new UnauthorizedBusinessException("Usuario o contraseña inválidos.");

        var usuario = resultadoDb.Value.Usuario;

        if (!usuario.Activo)
            throw new UnauthorizedBusinessException("El usuario se encuentra inactivo.");

        // 2. Verificar hash con las credenciales devueltas
        if (!VerificarPassword(request.Password, resultadoDb.Value.PasswordHash, resultadoDb.Value.PasswordSalt))
            throw new UnauthorizedBusinessException("Usuario o contraseña inválidos.");

        return UsuarioBusinessMapper.ToLoginResponse(usuario);
    }

    // -------------------------------------------------------------------------
    // Helper privado — HMACSHA256 con el salt almacenado
    // -------------------------------------------------------------------------
    private static bool VerificarPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        using var hmac = new System.Security.Cryptography.HMACSHA256(saltBytes);
        var computedHash = Convert.ToBase64String(
            hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        return computedHash == storedHash;
    }
}