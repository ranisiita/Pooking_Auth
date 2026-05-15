using Grpc.Core;
using Microservicio.Pooking.Auth.Api.Protos;
using Microservicio.Pooking.Auth.Business.Interfaces;

namespace Microservicio.Pooking.Auth.Api.Services;

public class AuthGrpcService : AuthGrpc.AuthGrpcBase
{
    private readonly IUsuarioService _usuarioService;

    public AuthGrpcService(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public override async Task<UsuarioGrpcReply> ObtenerUsuarioPorGuid(
        UsuarioGuidRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.GuidUsuario, out var guid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "El GUID proporcionado no es válido."));
        }

        try
        {
            var usuario = await _usuarioService.ObtenerPorGuidConRolesAsync(guid, context.CancellationToken);
            
            if (usuario is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Usuario con GUID {guid} no encontrado."));
            }

            var reply = new UsuarioGrpcReply
            {
                GuidUsuario = usuario.UsuarioGuid.ToString(),
                Username = usuario.Username ?? string.Empty,
                Correo = usuario.Correo ?? string.Empty,
                EstadoUsuario = usuario.EstadoUsuario ?? string.Empty,
                Activo = usuario.Activo
            };

            if (usuario.Roles != null)
            {
                reply.Roles.AddRange(usuario.Roles);
            }

            return reply;
        }
        catch (Microservicio.Pooking.Auth.Business.Exceptions.NotFoundException)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Usuario con GUID {guid} no encontrado."));
        }
    }

    public override async Task<ValidacionUsuarioReply> ValidarUsuarioActivo(
        UsuarioGuidRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.GuidUsuario, out var guid))
        {
            return new ValidacionUsuarioReply 
            { 
                EsValido = false, 
                Mensaje = "El GUID proporcionado no es válido." 
            };
        }

        try
        {
            var usuario = await _usuarioService.ObtenerPorGuidConRolesAsync(guid, context.CancellationToken);
            
            if (usuario is null)
            {
                return new ValidacionUsuarioReply 
                { 
                    EsValido = false, 
                    Mensaje = "Usuario no encontrado." 
                };
            }

            if (!usuario.Activo)
            {
                return new ValidacionUsuarioReply 
                { 
                    EsValido = false, 
                    Mensaje = "El usuario se encuentra inactivo." 
                };
            }

            return new ValidacionUsuarioReply 
            { 
                EsValido = true, 
                Mensaje = "Usuario válido y activo." 
            };
        }
        catch (Microservicio.Pooking.Auth.Business.Exceptions.NotFoundException)
        {
            return new ValidacionUsuarioReply 
            { 
                EsValido = false, 
                Mensaje = "Usuario no encontrado." 
            };
        }
    }
}
