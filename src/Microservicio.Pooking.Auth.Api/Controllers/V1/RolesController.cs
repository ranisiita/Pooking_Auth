using Asp.Versioning;
using Microservicio.Pooking.Auth.Api.Models.Common;
using Microservicio.Pooking.Auth.Business.DTOs.Rol;
using Microservicio.Pooking.Auth.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Pooking.Auth.Api.Controllers.V1;

/// <summary>
/// Gestión del catálogo de roles y asignaciones usuario-rol.
/// Todas las operaciones requieren rol ADMINISTRADOR excepto
/// la consulta del catálogo que puede verla cualquier usuario autenticado.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/roles")]
[Produces("application/json")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRolService _rolService;

    public RolesController(IRolService rolService)
    {
        _rolService = rolService;
    }

    // -------------------------------------------------------------------------
    // Catálogo de roles
    // -------------------------------------------------------------------------

    /// <summary>
    /// Lista todos los roles activos del sistema.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RolResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos(CancellationToken cancellationToken)
    {
        var result = await _rolService.ObtenerTodosAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RolResponse>>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>
    /// Obtiene un rol por su GUID público.
    /// </summary>
    [HttpGet("{rolGuid:guid}")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<RolResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorGuid(
        Guid rolGuid,
        CancellationToken cancellationToken)
    {
        var result = await _rolService.ObtenerPorGuidAsync(rolGuid, cancellationToken);
        return Ok(ApiResponse<RolResponse>.Ok(result!, "Consulta exitosa."));
    }

    /// <summary>
    /// Crea un nuevo rol en el catálogo. Solo ADMINISTRADOR.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<RolResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearRolRequest request,
        CancellationToken cancellationToken)
    {
        request.CreadoPorUsuario = ObtenerUsernameDelToken();

        var result = await _rolService.CrearAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(ObtenerPorGuid),
            new { rolGuid = result.RolGuid },
            ApiResponse<RolResponse>.Ok(result, "Rol creado exitosamente."));
    }

    /// <summary>
    /// Eliminación lógica de un rol. Solo ADMINISTRADOR.
    /// </summary>
    [HttpDelete("{rolGuid:guid}")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarLogico(
        Guid rolGuid,
        CancellationToken cancellationToken)
    {
        var modificadoPor = ObtenerUsernameDelToken();
        await _rolService.EliminarLogicoAsync(rolGuid, modificadoPor, cancellationToken);

        return Ok(ApiResponse<string>.Ok("OK", "Rol eliminado lógicamente."));
    }

    // -------------------------------------------------------------------------
    // Asignaciones usuario-rol
    // -------------------------------------------------------------------------

    /// <summary>
    /// Lista los roles asignados a un usuario específico.
    /// </summary>
    [HttpGet("usuario/{usuarioGuid:guid}")]
    [Authorize(Roles = "ADMINISTRADOR,VENDEDOR,CLIENTE")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RolResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerRolesDeUsuario(
        Guid usuarioGuid,
        CancellationToken cancellationToken)
    {
        var validacion = ValidarAccesoAUsuario(usuarioGuid);
        if (validacion != null) return validacion;

        var result = await _rolService.ObtenerRolesDeUsuarioAsync(usuarioGuid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<RolResponse>>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>
    /// Lista el detalle completo de asignaciones usuario-rol (estado, fecha, auditoría).
    /// </summary>
    [HttpGet("usuario/{usuarioGuid:guid}/asignaciones")]
    [Authorize(Roles = "ADMINISTRADOR,VENDEDOR")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UsuarioRolResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerAsignacionesDeUsuario(
        Guid usuarioGuid,
        CancellationToken cancellationToken)
    {
        var validacion = ValidarAccesoAUsuario(usuarioGuid);
        if (validacion != null) return validacion;

        var result = await _rolService.ObtenerAsignacionesDeUsuarioAsync(usuarioGuid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UsuarioRolResponse>>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>
    /// Asigna un rol a un usuario. Solo ADMINISTRADOR.
    /// </summary>
    [HttpPost("asignar")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AsignarRol(
        [FromBody] AsignarRolRequest request,
        CancellationToken cancellationToken)
    {
        request.EjecutadoPorUsuario = ObtenerUsernameDelToken();
        await _rolService.AsignarRolAsync(request, cancellationToken);

        return Ok(ApiResponse<string>.Ok("OK", "Rol asignado exitosamente."));
    }

    /// <summary>
    /// Revoca un rol de un usuario. Solo ADMINISTRADOR.
    /// </summary>
    [HttpPost("revocar")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevocarRol(
        [FromBody] AsignarRolRequest request,
        CancellationToken cancellationToken)
    {
        request.EjecutadoPorUsuario = ObtenerUsernameDelToken();
        await _rolService.RevocarRolAsync(request, cancellationToken);

        return Ok(ApiResponse<string>.Ok("OK", "Rol revocado exitosamente."));
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
                "No tienes permisos para consultar la información de otro usuario.", 
                StatusCodes.Status403Forbidden);
            return StatusCode(StatusCodes.Status403Forbidden, error);
        }

        return null;
    }
}
