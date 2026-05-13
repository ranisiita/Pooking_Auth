using Microservicio.Pooking.Auth.Business.DTOs.Auth;
using Microservicio.Pooking.Auth.Business.DTOs.Usuario;
using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.Business.Mappers;

/// <summary>
/// Mapper de la capa de negocio.
/// Transforma entre DataModels (Capa 2) y DTOs (Capa 3).
/// La API nunca ve DataModels; solo ve DTOs de respuesta.
/// </summary>
public static class UsuarioBusinessMapper
{
    // -------------------------------------------------------------------------
    // DataModel → Response (para la API)
    // -------------------------------------------------------------------------

    public static UsuarioResponse ToResponse(UsuarioDataModel model)
    {
        return new UsuarioResponse
        {
            UsuarioGuid = model.UsuarioGuid,
            Username = model.Username,
            Correo = model.Correo,
            EstadoUsuario = model.EstadoUsuario,
            Activo = model.Activo,
            FechaRegistroUtc = model.FechaRegistroUtc,
            Roles = model.NombresRoles
        };
    }

    // -------------------------------------------------------------------------
    // CrearUsuarioRequest → DataModel (para persistir)
    // -------------------------------------------------------------------------

    public static UsuarioDataModel ToDataModel(CrearUsuarioRequest request)
    {
        return new UsuarioDataModel
        {
            Username = request.Username.Trim(),
            Correo = request.Correo.Trim().ToLower(),
            EstadoUsuario = "ACT",
            Activo = true,
            EsEliminado = false,
            CreadoPorUsuario = request.CreadoPorUsuario
        };
    }

    // -------------------------------------------------------------------------
    // DataModel → LoginResponse (para autenticación)
    // -------------------------------------------------------------------------

    public static LoginResponse ToLoginResponse(UsuarioDataModel model)
    {
        return new LoginResponse
        {
            UsuarioGuid = model.UsuarioGuid,
            Username = model.Username,
            Correo = model.Correo,
            Activo = model.Activo,
            Roles = model.NombresRoles
        };
    }
}