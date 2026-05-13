using Asp.Versioning;
using Microservicio.Pooking.Auth.Api.Models.Common;
using Microservicio.Pooking.Auth.Api.Models.Settings;
using Microservicio.Pooking.Auth.Business.DTOs.Auth;
using Microservicio.Pooking.Auth.Business.DTOs.Usuario;
using Microservicio.Pooking.Auth.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Microservicio.Pooking.Auth.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUsuarioService _usuarioService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IAuthService authService,
        IUsuarioService usuarioService,
        IOptions<JwtSettings> jwtOptions)
    {
        _authService = authService;
        _usuarioService = usuarioService;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        result.UsuarioGuid.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, result.Username),
            new(JwtRegisteredClaimNames.Email,      result.Correo),
        };

        claims.AddRange(result.Roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        result.Token = new JwtSecurityTokenHandler().WriteToken(token);
        result.ExpirationUtc = expiration;

        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login exitoso."));
    }

    [HttpPost("registro")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Registro(
    [FromBody] CrearUsuarioRequest request,
    CancellationToken cancellationToken)
    {
        request.CreadoPorUsuario = request.Correo;
        var usuarioCreado = await _usuarioService.CrearAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<UsuarioResponse>.Exitoso(
            usuarioCreado, "Usuario registrado exitosamente.", StatusCodes.Status201Created));
    }
}