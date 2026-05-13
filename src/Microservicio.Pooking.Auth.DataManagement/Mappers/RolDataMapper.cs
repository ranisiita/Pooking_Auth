using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.DataManagement.Mappers;

/// <summary>
/// Mapper estático entre RolEntity (capa 1) y RolDataModel (capa 2).
/// </summary>
public static class RolDataMapper
{
    public static RolDataModel ToDataModel(RolEntity entity)
    {
        return new RolDataModel
        {
            IdRol = entity.IdRol,
            RolGuid = entity.RolGuid,
            NombreRol = entity.NombreRol,
            DescripcionRol = entity.DescripcionRol,
            EstadoRol = entity.EstadoRol,
            EsEliminado = entity.EsEliminado,
            Activo = entity.Activo,
            FechaRegistroUtc = entity.FechaRegistroUtc,
            CreadoPorUsuario = entity.CreadoPorUsuario,
            ModificadoPorUsuario = entity.ModificadoPorUsuario,
            FechaModificacionUtc = entity.FechaModificacionUtc
        };
    }

    public static RolEntity ToEntity(RolDataModel model)
    {
        return new RolEntity
        {
            IdRol = model.IdRol,
            RolGuid = model.RolGuid,
            NombreRol = model.NombreRol,
            DescripcionRol = model.DescripcionRol,
            EstadoRol = model.EstadoRol,
            EsEliminado = model.EsEliminado,
            Activo = model.Activo,
            FechaRegistroUtc = model.FechaRegistroUtc,
            CreadoPorUsuario = model.CreadoPorUsuario,
            ModificadoPorUsuario = model.ModificadoPorUsuario,
            FechaModificacionUtc = model.FechaModificacionUtc
        };
    }
}