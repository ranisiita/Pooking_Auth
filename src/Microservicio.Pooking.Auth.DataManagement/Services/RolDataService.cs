using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microservicio.Pooking.Auth.DataManagement.Interfaces;
using Microservicio.Pooking.Auth.DataManagement.Mappers;
using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.DataManagement.Services;

/// <summary>
/// Implementación del servicio de datos para roles y asignaciones usuario-rol.
/// Coordina RolRepository a través del UnitOfWork y mapea entidades a modelos.
/// </summary>
public class RolDataService : IRolDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public RolDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // -------------------------------------------------------------------------
    // Lecturas — Rol
    // -------------------------------------------------------------------------

    public async Task<RolDataModel?> ObtenerRolPorGuidAsync(
        Guid rolGuid,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.RolRepository
            .ObtenerRolPorGuidAsync(rolGuid, cancellationToken);

        return entity is null ? null : RolDataMapper.ToDataModel(entity);
    }

    public async Task<IReadOnlyList<RolDataModel>> ObtenerTodosLosRolesAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.RolRepository
            .ObtenerTodosLosRolesAsync(cancellationToken);

        return entities.Select(RolDataMapper.ToDataModel).ToList();
    }

    public async Task<IReadOnlyList<UsuarioRolDataModel>> ObtenerAsignacionesDeUsuarioAsync(
    int idUsuario,
    CancellationToken cancellationToken = default)
    {
        var asignaciones = await _unitOfWork.RolRepository
            .ObtenerRolesDeUsuarioAsync(idUsuario, cancellationToken);

        return asignaciones
            .Select(UsuarioRolDataMapper.ToDataModel)
            .ToList();
    }

    // -------------------------------------------------------------------------
    // Verificaciones — Rol
    // -------------------------------------------------------------------------

    public async Task<bool> ExisteNombreRolAsync(
        string nombreRol,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.RolRepository
            .ExisteNombreRolAsync(nombreRol, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Escritura — Rol
    // -------------------------------------------------------------------------

    public async Task<RolDataModel> CrearRolAsync(
        RolDataModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = RolDataMapper.ToEntity(model);
        entity.FechaRegistroUtc = DateTime.UtcNow;

        await _unitOfWork.RolRepository.AgregarRolAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RolDataMapper.ToDataModel(entity);
    }

    public async Task<bool> EliminarLogicoRolAsync(
        int idRol,
        string modificadoPorUsuario,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.RolRepository
            .ObtenerRolParaActualizarAsync(idRol, cancellationToken);

        if (entity is null) return false;

        _unitOfWork.RolRepository.EliminarLogicoRol(entity);

        entity.ModificadoPorUsuario = modificadoPorUsuario;
        entity.FechaModificacionUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    // -------------------------------------------------------------------------
    // Lecturas — Asignaciones usuario-rol
    // -------------------------------------------------------------------------

    public async Task<IReadOnlyList<RolDataModel>> ObtenerRolesDeUsuarioAsync(
        int idUsuario,
        CancellationToken cancellationToken = default)
    {
        var asignaciones = await _unitOfWork.RolRepository
            .ObtenerRolesDeUsuarioAsync(idUsuario, cancellationToken);

        return asignaciones
            .Select(ur => RolDataMapper.ToDataModel(ur.Rol))
            .ToList();
    }

    public async Task<bool> UsuarioTieneRolAsync(
        int idUsuario,
        int idRol,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.RolRepository
            .UsuarioTieneRolAsync(idUsuario, idRol, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Escritura — Asignaciones usuario-rol
    // -------------------------------------------------------------------------

    public async Task AsignarRolAsync(
        int idUsuario,
        int idRol,
        string creadoPorUsuario,
        CancellationToken cancellationToken = default)
    {
        var asignacion = new UsuarioRolEntity
        {
            IdUsuario = idUsuario,
            IdRol = idRol,
            CreadoPorUsuario = creadoPorUsuario,
            FechaRegistroUtc = DateTime.UtcNow,
            EstadoUsuarioRol = "ACT",
            Activo = true,
            EsEliminado = false
        };

        await _unitOfWork.RolRepository.AgregarAsignacionAsync(asignacion, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RevocarRolAsync(
        int idUsuario,
        int idRol,
        string modificadoPorUsuario,
        CancellationToken cancellationToken = default)
    {
        var asignacion = await _unitOfWork.RolRepository
            .ObtenerAsignacionParaActualizarAsync(idUsuario, idRol, cancellationToken);

        if (asignacion is null) return false;

        _unitOfWork.RolRepository.EliminarLogicoAsignacion(asignacion);

        asignacion.ModificadoPorUsuario = modificadoPorUsuario;
        asignacion.FechaModificacionUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}