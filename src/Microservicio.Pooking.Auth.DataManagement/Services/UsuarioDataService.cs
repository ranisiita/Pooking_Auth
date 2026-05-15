using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microservicio.Pooking.Auth.DataManagement.Interfaces;
using Microservicio.Pooking.Auth.DataManagement.Mappers;
using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.DataManagement.Services;

/// <summary>
/// Implementación del servicio de datos de usuarios.
/// Coordina repositorios a través del UnitOfWork, mapea entidades
/// a modelos y centraliza el guardado transaccional.
/// No contiene reglas de negocio: eso pertenece a la capa 3.
/// </summary>
public class UsuarioDataService : IUsuarioDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // -------------------------------------------------------------------------
    // Lecturas
    // -------------------------------------------------------------------------

    public async Task<UsuarioDataModel?> ObtenerPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerPorGuidConRolesAsync(
                // Necesitamos por id: usamos ObtenerPorIdAsync y luego cargamos roles
                // via segundo query si hace falta, pero aquí usamos el método base.
                // Para mantener la carga de roles se hace en dos pasos:
                default, cancellationToken);

        // Estrategia: obtener por id sin roles, luego obtener con roles por guid
        var entityBase = await _unitOfWork.UsuarioRepository
            .ObtenerPorIdAsync(idUsuario, cancellationToken);

        if (entityBase is null) return null;

        var entityConRoles = await _unitOfWork.UsuarioRepository
            .ObtenerPorGuidConRolesAsync(entityBase.UsuarioGuid, cancellationToken);

        return entityConRoles is null ? null : UsuarioDataMapper.ToDataModel(entityConRoles);
    }

    public async Task<UsuarioDataModel?> ObtenerPorGuidAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerPorGuidConRolesAsync(usuarioGuid, cancellationToken);

        return entity is null ? null : UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel?> ObtenerPorGuidConRolesAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerPorGuidConRolesAsync(usuarioGuid, cancellationToken);

        return entity is null ? null : UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel?> ObtenerPorUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerPorUsernameAsync(username, cancellationToken);

        if (entity is null) return null;

        return UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel?> ObtenerPorCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerPorCorreoAsync(correo, cancellationToken);

        if (entity is null) return null;

        var entityConRoles = await _unitOfWork.UsuarioRepository
            .ObtenerPorGuidConRolesAsync(entity.UsuarioGuid, cancellationToken);

        return entityConRoles is null ? null : UsuarioDataMapper.ToDataModel(entityConRoles);
    }

    public async Task<DataPagedResult<UsuarioDataModel>> BuscarAsync(
    UsuarioFiltroDataModel filtro,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.UsuarioQueryRepository
            .BuscarUsuariosAsync(
                filtro.Termino, 
                filtro.EstadoUsuario, 
                filtro.NombreRol, 
                filtro.PaginaActual, 
                filtro.TamanioPagina, 
                cancellationToken);

        return DataPagedResult<UsuarioDataModel>.DesdeDal(
            result,
            dto => new UsuarioDataModel
            {
                UsuarioGuid = dto.UsuarioGuid,
                Username = dto.Username,
                Correo = dto.Correo,
                EstadoUsuario = dto.EstadoUsuario,
                Activo = dto.Activo,
                FechaRegistroUtc = dto.FechaRegistroUtc,
                NombresRoles = dto.NombresRoles
            });
    }

    // -------------------------------------------------------------------------
    // Verificaciones
    // -------------------------------------------------------------------------

    public async Task<bool> ExisteUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UsuarioRepository
            .ExisteUsernameAsync(username, cancellationToken);
    }

    public async Task<bool> ExisteCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UsuarioRepository
            .ExisteCorreoAsync(correo, cancellationToken);
    }

    public async Task<(bool ExisteUsername, bool ExisteCorreo, bool ExisteRol, int IdRol)> ValidarRegistroAsync(
        string username,
        string correo,
        string nombreRol,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UsuarioRepository
            .ValidarRegistroAsync(username, correo, nombreRol, cancellationToken);
    }

    public async Task<UsuarioDataModel?> ObtenerPorUsernameOCorreoAsync(
    string identificador,
    CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerPorUsernameOCorreoConRolesAsync(identificador, cancellationToken);

        return entity is null ? null : UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<(string PasswordHash, string PasswordSalt)?> ObtenerCredencialesParaAuthAsync(
        string identificador,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerPorUsernameOCorreoConRolesAsync(identificador, cancellationToken);

        if (entity is null || entity.EsEliminado)
            return null;

        return (entity.PasswordHash, entity.PasswordSalt);
    }

    public async Task<(UsuarioDataModel Usuario, string PasswordHash, string PasswordSalt)?> ObtenerParaAutenticacionAsync(
        string identificador,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerPorUsernameOCorreoConRolesAsync(identificador, cancellationToken);

        if (entity is null || entity.EsEliminado)
            return null;

        return (UsuarioDataMapper.ToDataModel(entity), entity.PasswordHash, entity.PasswordSalt);
    }


    // -------------------------------------------------------------------------
    // Escritura
    // -------------------------------------------------------------------------

    public async Task<UsuarioDataModel> CrearAsync(
        UsuarioDataModel model,
        string passwordHash,
        string passwordSalt,
        CancellationToken cancellationToken = default)
    {
        var entity = UsuarioDataMapper.ToEntity(model);

        // Los hashes solo se asignan aquí, nunca viajan en el DataModel
        entity.PasswordHash = passwordHash;
        entity.PasswordSalt = passwordSalt;
        entity.FechaRegistroUtc = DateTime.UtcNow;

        await _unitOfWork.UsuarioRepository.AgregarAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel?> ActualizarAsync(
        UsuarioDataModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerParaActualizarAsync(model.IdUsuario, cancellationToken);

        if (entity is null) return null;

        // Solo actualiza campos modificables — nunca hash ni guid
        entity.Username = model.Username;
        entity.Correo = model.Correo;
        entity.EstadoUsuario = model.EstadoUsuario;
        entity.Activo = model.Activo;
        entity.ModificadoPorUsuario = model.ModificadoPorUsuario;
        entity.FechaModificacionUtc = DateTime.UtcNow;

        _unitOfWork.UsuarioRepository.Actualizar(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<bool> EliminarLogicoAsync(
        int idUsuario,
        string modificadoPorUsuario,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UsuarioRepository
            .ObtenerParaActualizarAsync(idUsuario, cancellationToken);

        if (entity is null) return false;

        _unitOfWork.UsuarioRepository.EliminarLogico(entity);

        entity.ModificadoPorUsuario = modificadoPorUsuario;
        entity.FechaModificacionUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

}