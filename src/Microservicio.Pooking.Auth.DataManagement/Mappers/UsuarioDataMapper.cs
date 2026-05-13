using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.DataManagement.Mappers;

/// <summary>
/// Mapper estático entre UsuarioAppEntity (capa 1) y UsuarioDataModel (capa 2).
/// Desacopla la representación física de EF Core del modelo que
/// consumen las capas superiores.
/// </summary>
public static class UsuarioDataMapper
{
    /// <summary>
    /// Convierte una entidad de base de datos a modelo de datos.
    /// Nunca expone PasswordHash ni PasswordSalt.
    /// </summary>
    public static UsuarioDataModel ToDataModel(UsuarioAppEntity entity)
    {
        return new UsuarioDataModel
        {
            IdUsuario = entity.IdUsuario,
            UsuarioGuid = entity.UsuarioGuid,
            Username = entity.Username,
            Correo = entity.Correo,
            EstadoUsuario = entity.EstadoUsuario,
            EsEliminado = entity.EsEliminado,
            Activo = entity.Activo,
            FechaRegistroUtc = entity.FechaRegistroUtc,
            CreadoPorUsuario = entity.CreadoPorUsuario,
            ModificadoPorUsuario = entity.ModificadoPorUsuario,
            FechaModificacionUtc = entity.FechaModificacionUtc,
            NombresRoles = entity.UsuariosRoles
                                         .Where(ur => !ur.EsEliminado && ur.Activo)
                                         .Select(ur => ur.Rol.NombreRol)
                                         .ToList()
        };
    }

    /// <summary>
    /// Convierte un modelo de datos a entidad para operaciones de escritura.
    /// PasswordHash y PasswordSalt deben setearse por separado en la capa
    /// de negocio antes de persistir.
    /// </summary>
    public static UsuarioAppEntity ToEntity(UsuarioDataModel model)
    {
        return new UsuarioAppEntity
        {
            IdUsuario = model.IdUsuario,
            UsuarioGuid = model.UsuarioGuid,
            Username = model.Username,
            Correo = model.Correo,
            EstadoUsuario = model.EstadoUsuario,
            EsEliminado = model.EsEliminado,
            Activo = model.Activo,
            FechaRegistroUtc = model.FechaRegistroUtc,
            CreadoPorUsuario = model.CreadoPorUsuario,
            ModificadoPorUsuario = model.ModificadoPorUsuario,
            FechaModificacionUtc = model.FechaModificacionUtc
            // PasswordHash y PasswordSalt: asignados explícitamente
            // por la capa de negocio, nunca desde el modelo de datos.
        };
    }
}