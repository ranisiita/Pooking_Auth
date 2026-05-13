using Microservicio.Pooking.Auth.DataAccess.Repositories.Interfaces;
using Microservicio.Pooking.Auth.DataAccess.Common;
using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microservicio.Pooking.Auth.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Auth.DataAccess.Repositories;

/// <summary>
/// Implementación de IRolRepository.
/// Gestiona el catálogo de roles (booking.rol) y las asignaciones
/// usuario-rol (booking.usuarios_roles).
/// Nunca llama SaveChanges directamente; esa responsabilidad
/// pertenece a la unidad de trabajo de la capa superior.
/// </summary>
public class RolRepository : IRolRepository
{
    private readonly AuthDbContext _context;

    public RolRepository(AuthDbContext context)
    {
        _context = context;
    }

    // -------------------------------------------------------------------------
    // Queries base reutilizables
    // -------------------------------------------------------------------------

    /// <summary>
    /// Roles vigentes: no eliminados lógicamente.
    /// </summary>
    private IQueryable<RolEntity> QueryRolesVigentes =>
        _context.Roles.Where(r => !r.EsEliminado);

    /// <summary>
    /// Asignaciones vigentes: no eliminadas lógicamente y activas.
    /// </summary>
    private IQueryable<UsuarioRolEntity> QueryAsignacionesVigentes =>
        _context.UsuariosRoles.Where(ur => !ur.EsEliminado && ur.Activo);

    // -------------------------------------------------------------------------
    // Lecturas — Rol
    // -------------------------------------------------------------------------

    public async Task<RolEntity?> ObtenerRolPorIdAsync(
        int idRol,
        CancellationToken cancellationToken = default)
    {
        return await QueryRolesVigentes
            .FirstOrDefaultAsync(r => r.IdRol == idRol, cancellationToken);
    }

    public async Task<RolEntity?> ObtenerRolPorGuidAsync(
        Guid rolGuid,
        CancellationToken cancellationToken = default)
    {
        return await QueryRolesVigentes
            .FirstOrDefaultAsync(r => r.RolGuid == rolGuid, cancellationToken);
    }

    public async Task<RolEntity?> ObtenerRolPorNombreAsync(
        string nombreRol,
        CancellationToken cancellationToken = default)
    {
        return await QueryRolesVigentes
            .FirstOrDefaultAsync(
                r => r.NombreRol == nombreRol,
                cancellationToken);
    }

    public async Task<IReadOnlyList<RolEntity>> ObtenerTodosLosRolesAsync(
        CancellationToken cancellationToken = default)
    {
        return await QueryRolesVigentes
            .Where(r => r.EstadoRol == "ACT")
            .OrderBy(r => r.NombreRol)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<RolEntity>> ObtenerRolesPaginadoAsync(
        int paginaActual,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var query = QueryRolesVigentes.OrderBy(r => r.NombreRol);

        var totalRegistros = await query.CountAsync(cancellationToken);

        if (totalRegistros == 0)
            return PagedResult<RolEntity>.Vacio(paginaActual, tamanoPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<RolEntity>(
            items,
            totalRegistros,
            paginaActual,
            tamanoPagina);
    }

    public async Task<RolEntity?> ObtenerRolParaActualizarAsync(
    int idRol,
    CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.IdRol == idRol
                                   && !r.EsEliminado, cancellationToken);
    }

    public async Task<UsuarioRolEntity?> ObtenerAsignacionParaActualizarAsync(
        int idUsuario,
        int idRol,
        CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosRoles
            .FirstOrDefaultAsync(ur => ur.IdUsuario == idUsuario
                                    && ur.IdRol == idRol
                                    && !ur.EsEliminado, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Verificaciones — Rol
    // -------------------------------------------------------------------------

    public async Task<bool> ExisteNombreRolAsync(
        string nombreRol,
        CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AnyAsync(r => r.NombreRol == nombreRol, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Escritura — Rol
    // -------------------------------------------------------------------------

    public async Task AgregarRolAsync(
        RolEntity rol,
        CancellationToken cancellationToken = default)
    {
        await _context.Roles.AddAsync(rol, cancellationToken);
    }

    public void ActualizarRol(RolEntity rol)
    {
        _context.Roles.Update(rol);
    }

    public void EliminarLogicoRol(RolEntity rol)
    {
        rol.EsEliminado = true;
        rol.EstadoRol = "INA";
        rol.Activo = false;

        _context.Roles.Update(rol);
    }

    // -------------------------------------------------------------------------
    // Lecturas — UsuarioRol (asignaciones)
    // -------------------------------------------------------------------------

    public async Task<IReadOnlyList<UsuarioRolEntity>> ObtenerRolesDeUsuarioAsync(
        int idUsuario,
        CancellationToken cancellationToken = default)
    {
        return await QueryAsignacionesVigentes
            .Where(ur => ur.IdUsuario == idUsuario)
            .Include(ur => ur.Rol)
            .OrderBy(ur => ur.Rol.NombreRol)
            .ToListAsync(cancellationToken);
    }

    public async Task<UsuarioRolEntity?> ObtenerAsignacionAsync(
        int idUsuario,
        int idRol,
        CancellationToken cancellationToken = default)
    {
        return await QueryAsignacionesVigentes
            .FirstOrDefaultAsync(
                ur => ur.IdUsuario == idUsuario && ur.IdRol == idRol,
                cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Verificaciones — UsuarioRol
    // -------------------------------------------------------------------------

    public async Task<bool> UsuarioTieneRolAsync(
        int idUsuario,
        int idRol,
        CancellationToken cancellationToken = default)
    {
        return await QueryAsignacionesVigentes
            .AnyAsync(
                ur => ur.IdUsuario == idUsuario && ur.IdRol == idRol,
                cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Escritura — UsuarioRol (asignaciones)
    // -------------------------------------------------------------------------

    public async Task AgregarAsignacionAsync(
        UsuarioRolEntity usuarioRol,
        CancellationToken cancellationToken = default)
    {
        await _context.UsuariosRoles.AddAsync(usuarioRol, cancellationToken);
    }

    public void EliminarLogicoAsignacion(UsuarioRolEntity usuarioRol)
    {
        usuarioRol.EsEliminado = true;
        usuarioRol.EstadoUsuarioRol = "INA";
        usuarioRol.Activo = false;

        _context.UsuariosRoles.Update(usuarioRol);
    }
}