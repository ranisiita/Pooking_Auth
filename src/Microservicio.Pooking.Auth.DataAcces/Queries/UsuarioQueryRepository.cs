using Microservicio.Pooking.Auth.DataAccess.Common;
using Microservicio.Pooking.Auth.DataAccess.Context;
using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Auth.DataAccess.Queries;

// =============================================================================
// DTOs de proyección — viven aquí porque son contratos de solo lectura
// propios de la DAL. No son entidades ni modelos de negocio.
// =============================================================================

/// <summary>
/// Proyección plana de un usuario con sus roles para listados y búsquedas.
/// Evita exponer PasswordHash / PasswordSalt fuera de la DAL.
/// </summary>
public sealed record UsuarioResumenDto(
    Guid UsuarioGuid,
    string Username,
    string Correo,
    string EstadoUsuario,
    bool Activo,
    DateTime FechaRegistroUtc,
    IReadOnlyList<string> NombresRoles
);

/// <summary>
/// Proyección detallada de un usuario para vistas de administración.
/// </summary>
public sealed record UsuarioDetalleDto(
    Guid UsuarioGuid,
    string Username,
    string Correo,
    string EstadoUsuario,
    bool Activo,
    bool EsEliminado,
    DateTime FechaRegistroUtc,
    string CreadoPorUsuario,
    string? ModificadoPorUsuario,
    DateTime? FechaModificacionUtc,
    IReadOnlyList<RolResumenDto> Roles
);

/// <summary>
/// Proyección mínima de un rol para incluir dentro de otros DTOs.
/// </summary>
public sealed record RolResumenDto(
    Guid RolGuid,
    string NombreRol,
    string EstadoRol
);

// =============================================================================
// Repositorio de consultas
// =============================================================================

/// <summary>
/// Repositorio de solo lectura para el dominio de usuarios.
/// Aplica el lado Query del patrón CQRS liviano:
///   - Nunca modifica estado.
///   - Usa proyecciones (Select) para no cargar entidades completas.
///   - AsNoTracking() en todas las consultas para máximo rendimiento.
/// Las consultas complejas multi-entidad que no encajan en un repositorio
/// de escritura específico viven aquí.
/// </summary>
public class UsuarioQueryRepository
{
    private readonly AuthDbContext _context;

    public UsuarioQueryRepository(AuthDbContext context)
    {
        _context = context;
    }

    // -------------------------------------------------------------------------
    // Query base — reutilizable internamente
    // -------------------------------------------------------------------------

    private IQueryable<UsuarioAppEntity> QueryVigentes =>
        _context.UsuariosApp
                .AsNoTracking()
                .Where(u => !u.EsEliminado);

    // -------------------------------------------------------------------------
    // Búsqueda y listado
    // -------------------------------------------------------------------------

    // -------------------------------------------------------------------------

    /// <summary>
    /// Retorna el detalle completo de un usuario por su GUID público,
    /// incluyendo todos sus roles activos con información de auditoría.
    /// Retorna null si el usuario no existe o está eliminado lógicamente.
    /// </summary>
    public async Task<UsuarioDetalleDto?> ObtenerDetalleAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default)
    {
        return await QueryVigentes
            .Where(u => u.UsuarioGuid == usuarioGuid)
            .Select(u => new UsuarioDetalleDto(
                u.UsuarioGuid,
                u.Username,
                u.Correo,
                u.EstadoUsuario,
                u.Activo,
                u.EsEliminado,
                u.FechaRegistroUtc,
                u.CreadoPorUsuario,
                u.ModificadoPorUsuario,
                u.FechaModificacionUtc,
                u.UsuariosRoles
                  .Where(ur => !ur.EsEliminado && ur.Activo)
                  .Select(ur => new RolResumenDto(
                      ur.Rol.RolGuid,
                      ur.Rol.NombreRol,
                      ur.Rol.EstadoRol))
                  .ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Búsqueda de usuarios por término parcial sobre username o correo.
    /// Útil para autocompletar o filtros en paneles de administración.
    /// </summary>
    public async Task<PagedResult<UsuarioResumenDto>> BuscarUsuariosAsync(
        string? termino,
        string? estadoUsuario,
        string? nombreRol,
        int paginaActual,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var query = QueryVigentes;

        if (!string.IsNullOrWhiteSpace(termino))
        {
            var terminoLower = termino.Trim().ToLower();
            query = query.Where(u =>
                u.Username.ToLower().Contains(terminoLower) ||
                u.Correo.ToLower().Contains(terminoLower));
        }

        if (!string.IsNullOrWhiteSpace(estadoUsuario))
        {
            query = query.Where(u => u.EstadoUsuario == estadoUsuario);
        }

        if (!string.IsNullOrWhiteSpace(nombreRol))
        {
            var rolLower = nombreRol.Trim().ToLower();
            query = query.Where(u => u.UsuariosRoles.Any(ur => 
                ur.Rol.NombreRol.ToLower() == rolLower && 
                !ur.EsEliminado && 
                ur.Activo));
        }

        query = query.OrderBy(u => u.Username);

        var totalRegistros = await query.CountAsync(cancellationToken);

        if (totalRegistros == 0)
            return PagedResult<UsuarioResumenDto>.Vacio(paginaActual, tamanoPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(u => new UsuarioResumenDto(
                u.UsuarioGuid,
                u.Username,
                u.Correo,
                u.EstadoUsuario,
                u.Activo,
                u.FechaRegistroUtc,
                u.UsuariosRoles
                  .Where(ur => !ur.EsEliminado && ur.Activo)
                  .Select(ur => ur.Rol.NombreRol)
                  .ToList()
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<UsuarioResumenDto>(
            items,
            totalRegistros,
            paginaActual,
            tamanoPagina);
    }

    /// <summary>
    /// Retorna todos los usuarios que tienen asignado un rol específico.
    /// Útil para auditorías de seguridad y gestión de permisos masiva.
    /// </summary>
    public async Task<IReadOnlyList<UsuarioResumenDto>> ListarUsuariosPorRolAsync(
        Guid rolGuid,
        CancellationToken cancellationToken = default)
    {
        return await QueryVigentes
            .Where(u => u.UsuariosRoles.Any(ur =>
                ur.Rol.RolGuid == rolGuid &&
                !ur.EsEliminado &&
                ur.Activo))
            .OrderBy(u => u.Username)
            .Select(u => new UsuarioResumenDto(
                u.UsuarioGuid,
                u.Username,
                u.Correo,
                u.EstadoUsuario,
                u.Activo,
                u.FechaRegistroUtc,
                u.UsuariosRoles
                  .Where(ur => !ur.EsEliminado && ur.Activo)
                  .Select(ur => ur.Rol.NombreRol)
                  .ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}