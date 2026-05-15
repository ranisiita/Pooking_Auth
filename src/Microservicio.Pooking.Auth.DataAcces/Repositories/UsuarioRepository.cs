
using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microservicio.Pooking.Auth.DataAccess.Repositories.Interfaces;
using Microservicio.Pooking.Auth.DataAccess.Common;
using Microservicio.Pooking.Auth.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Auth.DataAccess.Repositories;

/// <summary>
/// Implementación de IUsuarioRepository.
/// Toda operación de escritura requiere que el llamador invoque
/// SaveChangesAsync en la unidad de trabajo (UoW) de la capa superior.
/// Este repositorio nunca llama SaveChanges directamente.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AuthDbContext _context;

    public UsuarioRepository(AuthDbContext context)
    {
        _context = context;
    }

    // -------------------------------------------------------------------------
    // Query base reutilizable — filtra eliminados lógicos en un solo lugar
    // -------------------------------------------------------------------------

    /// <summary>
    /// Punto de partida para todas las consultas de usuario.
    /// Excluye registros con borrado lógico aplicado (es_eliminado = true).
    /// </summary>
    private IQueryable<UsuarioAppEntity> QueryVigentes =>
        _context.UsuariosApp.Where(u => !u.EsEliminado);

    // -------------------------------------------------------------------------
    // Lecturas simples
    // -------------------------------------------------------------------------

    public async Task<UsuarioAppEntity?> ObtenerPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken = default)
    {
        return await QueryVigentes
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario, cancellationToken);
    }

    public async Task<UsuarioAppEntity?> ObtenerPorGuidAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default)
    {
        return await QueryVigentes
            .FirstOrDefaultAsync(u => u.UsuarioGuid == usuarioGuid, cancellationToken);
    }

    public async Task<UsuarioAppEntity?> ObtenerPorUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        return await QueryVigentes
            .Include(u => u.UsuariosRoles.Where(ur => !ur.EsEliminado && ur.Activo))
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(
                u => u.Username == username,
                cancellationToken);
    }

    public async Task<UsuarioAppEntity?> ObtenerPorCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default)
    {
        return await QueryVigentes
            .FirstOrDefaultAsync(
                u => u.Correo == correo,
                cancellationToken);
    }

    public async Task<UsuarioAppEntity?> ObtenerParaActualizarAsync(
    int idUsuario,
    CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosApp
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario
                                   && !u.EsEliminado, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Lecturas con roles
    // -------------------------------------------------------------------------

    public async Task<UsuarioAppEntity?> ObtenerPorGuidConRolesAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default)
    {
        return await QueryVigentes
            .Include(u => u.UsuariosRoles.Where(ur => !ur.EsEliminado && ur.Activo))
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.UsuarioGuid == usuarioGuid, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Lecturas paginadas
    // -------------------------------------------------------------------------

    public async Task<PagedResult<UsuarioAppEntity>> ObtenerTodosPaginadoAsync(
        int paginaActual,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var query = QueryVigentes.OrderBy(u => u.IdUsuario);

        var totalRegistros = await query.CountAsync(cancellationToken);

        if (totalRegistros == 0)
            return PagedResult<UsuarioAppEntity>.Vacio(paginaActual, tamanoPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<UsuarioAppEntity>(
            items,
            totalRegistros,
            paginaActual,
            tamanoPagina);
    }

    // -------------------------------------------------------------------------
    // Verificaciones
    // -------------------------------------------------------------------------

    public async Task<bool> ExisteUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosApp
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ExisteCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosApp
            .AnyAsync(u => u.Correo == correo, cancellationToken);
    }

    public async Task<(bool ExisteUsername, bool ExisteCorreo, bool ExisteRol, int IdRol)> ValidarRegistroAsync(
        string username,
        string correo,
        string nombreRol,
        CancellationToken cancellationToken = default)
    {
        // En EF Core, proyectamos desde la tabla de Roles (que sabemos que tiene registros) 
        // para asegurar que siempre haya un resultado, aún si la tabla UsuariosApp está vacía.
        var result = await _context.Roles
            .Select(r => new
            {
                ExisteUsername = _context.UsuariosApp.Any(u => u.Username == username),
                ExisteCorreo = _context.UsuariosApp.Any(u => u.Correo == correo),
                RolInfo = _context.Roles.Where(x => x.NombreRol == nombreRol).Select(x => x.IdRol).FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
            return (false, false, false, 0);

        return (
            result.ExisteUsername,
            result.ExisteCorreo,
            result.RolInfo > 0,
            result.RolInfo
        );
    }

    public async Task<UsuarioAppEntity?> ObtenerPorUsernameOCorreoConRolesAsync(
        string identificador,
        CancellationToken cancellationToken = default)
    {
        var id = identificador.Trim().ToLower();

        return await QueryVigentes
            .Include(u => u.UsuariosRoles.Where(ur => !ur.EsEliminado && ur.Activo))
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(
                u => u.Username.ToLower() == id || u.Correo.ToLower() == id,
                cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Escritura
    // -------------------------------------------------------------------------

    public async Task AgregarAsync(
        UsuarioAppEntity usuario,
        CancellationToken cancellationToken = default)
    {
        await _context.UsuariosApp.AddAsync(usuario, cancellationToken);
    }

    public void Actualizar(UsuarioAppEntity usuario)
    {
        // EF Core rastrea el objeto si fue obtenido en el mismo contexto.
        // Update() fuerza el tracking en caso de que sea una entidad desconectada.
        _context.UsuariosApp.Update(usuario);
    }

    public void EliminarLogico(UsuarioAppEntity usuario)
    {
        usuario.EsEliminado = true;
        usuario.EstadoUsuario = "INA";
        usuario.Activo = false;

        // EF Core detecta el cambio automáticamente si la entidad está tracked.
        // No es necesario llamar Update() explícitamente en este caso.
        _context.UsuariosApp.Update(usuario);
    }
}