using Asp.Versioning;
using Microservicio.Pooking.Auth.Api.Models.Common;
using Microservicio.Pooking.Auth.Business.DTOs.Usuario;
using Microservicio.Pooking.Auth.Business.Interfaces;
using Microservicio.Pooking.Auth.DataManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Pooking.Auth.Api.Controllers.V1;

/// <summary>
/// CRUD de usuarios del sistema.
/// Requiere autenticación JWT. Las operaciones de eliminación
/// están restringidas al rol ADMINISTRADOR.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/usuarios")]
[Produces("application/json")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    // -------------------------------------------------------------------------
    // GET /api/v1/usuarios/{guid}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Obtiene un usuario por su GUID público.
    /// </summary>
    [HttpGet("{usuarioGuid:guid}")]
    [Authorize(Roles = "ADMINISTRADOR,VENDEDOR,CLIENTE")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorGuid(
        Guid usuarioGuid,
        CancellationToken cancellationToken)
    {
        var validacion = ValidarAccesoAUsuario(usuarioGuid);
        if (validacion != null) return validacion;

        var result = await _usuarioService.ObtenerPorGuidAsync(usuarioGuid, cancellationToken);
        return Ok(ApiResponse<UsuarioResponse>.Ok(result!, "Consulta exitosa."));
    }

    // -------------------------------------------------------------------------
    // GET /api/v1/usuarios/disponibilidad/{username}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifica si un username está disponible.
    /// Este endpoint es público para validaciones desde el frontend durante el registro.
    /// </summary>
    [HttpGet("disponibilidad/{username}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerificarDisponibilidad(string username, CancellationToken ct)
    {
        var disponible = await _usuarioService.UsernameDisponibleAsync(username, ct);
        return Ok(ApiResponse<object>.Ok(new { username, disponible },
            disponible ? "Username disponible." : "Username ya está en uso."));
    }

    // -------------------------------------------------------------------------
    // GET /api/v1/usuarios/disponibilidad-correo/{correo}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifica si un correo electrónico está disponible.
    /// Este endpoint es público para validaciones desde el frontend durante el registro.
    /// </summary>
    [HttpGet("disponibilidad-correo/{correo}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerificarDisponibilidadCorreo(string correo, CancellationToken ct)
    {
        var disponible = await _usuarioService.CorreoDisponibleAsync(correo, ct);
        return Ok(ApiResponse<object>.Ok(new { correo, disponible },
            disponible ? "Correo disponible." : "Correo ya está en uso."));
    }

    // -------------------------------------------------------------------------
    // POST /api/v1/usuarios/buscar
    // -------------------------------------------------------------------------

    /// <summary>
    /// Búsqueda paginada de usuarios con filtros opcionales.
    /// </summary>
    [HttpPost("buscar")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<DataPagedResult<UsuarioResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Buscar(
        [FromBody] UsuarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var result = await _usuarioService.BuscarAsync(filtro, cancellationToken);
        return Ok(ApiResponse<DataPagedResult<UsuarioResponse>>.Ok(result, "Consulta paginada exitosa."));
    }

    // -------------------------------------------------------------------------
    // POST /api/v1/usuarios
    // -------------------------------------------------------------------------

    /// <summary>
    /// Registra un nuevo usuario y le asigna el rol indicado automáticamente.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        // Auditoría: quién ejecuta la operación se toma del token, no del body
        request.CreadoPorUsuario = ObtenerUsernameDelToken();

        var result = await _usuarioService.CrearAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(ObtenerPorGuid),
            new { usuarioGuid = result.UsuarioGuid },
            ApiResponse<UsuarioResponse>.Ok(result, "Usuario creado exitosamente."));
    }

    // -------------------------------------------------------------------------
    // PUT /api/v1/usuarios/{guid}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Actualiza username y correo de un usuario existente.
    /// </summary>
    [HttpPut("{usuarioGuid:guid}")]
    [Authorize(Roles = "ADMINISTRADOR,VENDEDOR,CLIENTE")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(
        Guid usuarioGuid,
        [FromBody] ActualizarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var validacion = ValidarAccesoAUsuario(usuarioGuid);
        if (validacion != null) return validacion;

        request.UsuarioGuid = usuarioGuid;
        request.ModificadoPorUsuario = ObtenerUsernameDelToken();

        var result = await _usuarioService.ActualizarAsync(request, cancellationToken);
        return Ok(ApiResponse<UsuarioResponse>.Ok(result, "Usuario actualizado exitosamente."));
    }

    // -------------------------------------------------------------------------
    // DELETE /api/v1/usuarios/{guid}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Eliminación lógica de un usuario. Solo ADMINISTRADOR puede ejecutarla.
    /// </summary>
    [HttpDelete("{usuarioGuid:guid}")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarLogico(
        Guid usuarioGuid,
        CancellationToken cancellationToken)
    {
        var modificadoPor = ObtenerUsernameDelToken();
        await _usuarioService.EliminarLogicoAsync(usuarioGuid, modificadoPor, cancellationToken);

        return Ok(ApiResponse<string>.Ok("OK", "Usuario eliminado lógicamente."));
    }

    // -------------------------------------------------------------------------
    // Helper privado
    // -------------------------------------------------------------------------

    private string ObtenerUsernameDelToken()
    {
        return User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName)?.Value
            ?? User.Identity?.Name
            ?? "sistema";
    }

    private IActionResult? ValidarAccesoAUsuario(Guid usuarioGuid)
    {
        if (User.IsInRole("ADMINISTRADOR"))
            return null;

        var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? User.FindFirst("id")?.Value
                      ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        if (claimId != usuarioGuid.ToString())
        {
            var error = ApiErrorResponse.Crear(
                "No tienes permisos para acceder o modificar la información de otro usuario.", 
                StatusCodes.Status403Forbidden);
            return StatusCode(StatusCodes.Status403Forbidden, error);
        }

        return null;
    }
}
