using Microservicio.Pooking.Auth.DataManagement.Models;
using Microservicio.Pooking.Auth.DataAccess.Entities;


namespace Microservicio.Pooking.Auth.DataManagement.Mappers;

/// <summary>
/// Mapper entre UsuarioRolEntity (Capa 1) y UsuarioRolDataModel (Capa 2).
/// </summary>
public static class UsuarioRolDataMapper
{
    public static UsuarioRolDataModel ToDataModel(UsuarioRolEntity entity)
    {
        return new UsuarioRolDataModel
        {
            IdUsuarioRol = entity.IdUsuarioRol,
            IdUsuario = entity.IdUsuario,
            IdRol = entity.IdRol,
            EstadoUsuarioRol = entity.EstadoUsuarioRol,
            EsEliminado = entity.EsEliminado,
            Activo = entity.Activo,
            FechaRegistroUtc = entity.FechaRegistroUtc,
            CreadoPorUsuario = entity.CreadoPorUsuario,
            ModificadoPorUsuario = entity.ModificadoPorUsuario,
            FechaModificacionUtc = entity.FechaModificacionUtc,
            // Datos navegados: solo se populan si la entidad tiene Include cargado
            NombreRol = entity.Rol?.NombreRol ?? string.Empty,
            Username = entity.Usuario?.Username ?? string.Empty
        };
    }

    public static UsuarioRolEntity ToEntity(UsuarioRolDataModel model)
    {
        return new UsuarioRolEntity
        {
            IdUsuarioRol = model.IdUsuarioRol,
            IdUsuario = model.IdUsuario,
            IdRol = model.IdRol,
            EstadoUsuarioRol = model.EstadoUsuarioRol,
            EsEliminado = model.EsEliminado,
            Activo = model.Activo,
            FechaRegistroUtc = model.FechaRegistroUtc,
            CreadoPorUsuario = model.CreadoPorUsuario,
            ModificadoPorUsuario = model.ModificadoPorUsuario,
            FechaModificacionUtc = model.FechaModificacionUtc
        };
    }
}