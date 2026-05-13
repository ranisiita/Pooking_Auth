using Microservicio.Pooking.Auth.Business.DTOs.Rol;
using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.Business.Mappers;

/// <summary>
/// Mapper de la capa de negocio para roles y asignaciones usuario-rol.
/// Transforma entre DataModels (Capa 2) y DTOs de Rol (Capa 3).
/// </summary>
public static class RolBusinessMapper
{
    // -------------------------------------------------------------------------
    // RolDataModel → RolResponse
    // -------------------------------------------------------------------------

    public static RolResponse ToResponse(RolDataModel model)
    {
        return new RolResponse
        {
            RolGuid = model.RolGuid,
            NombreRol = model.NombreRol,
            DescripcionRol = model.DescripcionRol,
            EstadoRol = model.EstadoRol,
            Activo = model.Activo
        };
    }

    // -------------------------------------------------------------------------
    // CrearRolRequest → RolDataModel
    // -------------------------------------------------------------------------

    public static RolDataModel ToDataModel(CrearRolRequest request)
    {
        return new RolDataModel
        {
            NombreRol = request.NombreRol.Trim().ToUpper(),
            DescripcionRol = request.DescripcionRol?.Trim(),
            EstadoRol = "ACT",
            Activo = true,
            EsEliminado = false,
            CreadoPorUsuario = request.CreadoPorUsuario
        };
    }

    // -------------------------------------------------------------------------
    // UsuarioRolDataModel → UsuarioRolResponse
    // -------------------------------------------------------------------------

    public static UsuarioRolResponse ToUsuarioRolResponse(UsuarioRolDataModel model)
    {
        return new UsuarioRolResponse
        {
            Username = model.Username,
            NombreRol = model.NombreRol,
            EstadoUsuarioRol = model.EstadoUsuarioRol,
            Activo = model.Activo,
            FechaRegistroUtc = model.FechaRegistroUtc,
            CreadoPorUsuario = model.CreadoPorUsuario
        };
    }
}