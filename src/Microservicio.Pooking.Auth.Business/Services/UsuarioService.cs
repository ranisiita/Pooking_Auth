using Microservicio.Pooking.Auth.Business.DTOs.Usuario;
using Microservicio.Pooking.Auth.Business.Exceptions;
using Microservicio.Pooking.Auth.Business.Interfaces;
using Microservicio.Pooking.Auth.Business.Mappers;
using Microservicio.Pooking.Auth.Business.Validators;
using Microservicio.Pooking.Auth.DataManagement.Interfaces;
using Microservicio.Pooking.Auth.DataManagement.Models;

namespace Microservicio.Pooking.Auth.Business.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioDataService _usuarioDataService;
    private readonly IRolDataService _rolDataService;

    public UsuarioService(
        IUsuarioDataService usuarioDataService,
        IRolDataService rolDataService)
    {
        _usuarioDataService = usuarioDataService;
        _rolDataService = rolDataService;
    }

    public async Task<UsuarioResponse?> ObtenerPorGuidAsync(
        Guid usuarioGuid,
        CancellationToken cancellationToken = default)
    {
        var model = await _usuarioDataService
            .ObtenerPorGuidAsync(usuarioGuid, cancellationToken);

        if (model is null)
            throw new NotFoundException($"No se encontró el usuario con GUID '{usuarioGuid}'.");

        return UsuarioBusinessMapper.ToResponse(model);
    }

    public async Task<DataPagedResult<UsuarioResponse>> BuscarAsync(
        UsuarioFiltroRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var filtroDataModel = new UsuarioFiltroDataModel
        {
            Termino = filtro.Termino,
            EstadoUsuario = filtro.EstadoUsuario,
            NombreRol = filtro.NombreRol,
            PaginaActual = filtro.PageNumber,
            TamanioPagina = filtro.PageSize
        };

        var resultado = await _usuarioDataService
            .BuscarAsync(filtroDataModel, cancellationToken);

        var items = resultado.Items
                             .Select(UsuarioBusinessMapper.ToResponse)
                             .ToList();

        return new DataPagedResult<UsuarioResponse>(
            items,
            resultado.TotalRegistros,
            resultado.PaginaActual,
            resultado.TamanoPagina);
    }

    public async Task<bool> UsernameDisponibleAsync(string username, CancellationToken ct = default)
    {
        var existe = await _usuarioDataService.ExisteUsernameAsync(username, ct);
        return !existe; // true = disponible, false = ocupado
    }

    public async Task<bool> CorreoDisponibleAsync(string correo, CancellationToken ct = default)
    {
        var existe = await _usuarioDataService.ExisteCorreoAsync(correo, ct);
        return !existe; // true = disponible, false = ocupado
    }

    public async Task<UsuarioResponse> CrearAsync(
        CrearUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        UsuarioValidator.ValidarCrear(request);

        // 1. Unificar las validaciones de lectura en una sola llamada (Evita Bug Npgsql Pool)
        var validacion = await _usuarioDataService.ValidarRegistroAsync(request.Username, request.Correo, request.NombreRol, cancellationToken);

        if (validacion.ExisteUsername)
            throw new ValidationException($"El username '{request.Username}' ya está en uso.");

        if (validacion.ExisteCorreo)
            throw new ValidationException($"El correo '{request.Correo}' ya está registrado.");

        if (!validacion.ExisteRol)
            throw new NotFoundException($"No se encontró el rol '{request.NombreRol}'.");

        var (hash, salt) = GenerarPasswordHash(request.Password);
        var dataModel = UsuarioBusinessMapper.ToDataModel(request);
        
        // 2. Crear usuario
        var usuarioCreado = await _usuarioDataService
            .CrearAsync(dataModel, hash, salt, cancellationToken);

        // 3. Asignar rol usando el IdRol obtenido en la validación
        await _rolDataService.AsignarRolAsync(
            usuarioCreado.IdUsuario,
            validacion.IdRol,
            request.CreadoPorUsuario,
            cancellationToken);

        // 4. Retornar respuesta
        usuarioCreado.NombresRoles = new List<string> { request.NombreRol };
        return UsuarioBusinessMapper.ToResponse(usuarioCreado);
    }

    public async Task<UsuarioResponse> ActualizarAsync(
        ActualizarUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        UsuarioValidator.ValidarActualizar(request);

        var existente = await _usuarioDataService
            .ObtenerPorGuidAsync(request.UsuarioGuid, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el usuario con GUID '{request.UsuarioGuid}'.");

        if (!string.Equals(existente.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            if (await _usuarioDataService.ExisteUsernameAsync(request.Username, cancellationToken))
                throw new ValidationException($"El username '{request.Username}' ya está en uso.");
        }

        if (!string.Equals(existente.Correo, request.Correo, StringComparison.OrdinalIgnoreCase))
        {
            if (await _usuarioDataService.ExisteCorreoAsync(request.Correo, cancellationToken))
                throw new ValidationException($"El correo '{request.Correo}' ya está registrado.");
        }

        existente.Username = request.Username.Trim();
        existente.Correo = request.Correo.Trim().ToLower();
        existente.ModificadoPorUsuario = request.ModificadoPorUsuario;

        var actualizado = await _usuarioDataService
            .ActualizarAsync(existente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el usuario con GUID '{request.UsuarioGuid}'.");

        return UsuarioBusinessMapper.ToResponse(actualizado);
    }

    public async Task EliminarLogicoAsync(
        Guid usuarioGuid,
        string modificadoPorUsuario,
        CancellationToken cancellationToken = default)
    {
        var existente = await _usuarioDataService
            .ObtenerPorGuidAsync(usuarioGuid, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el usuario con GUID '{usuarioGuid}'.");

        var eliminado = await _usuarioDataService
            .EliminarLogicoAsync(existente.IdUsuario, modificadoPorUsuario, cancellationToken);

        if (!eliminado)
            throw new NotFoundException($"No se pudo eliminar el usuario con GUID '{usuarioGuid}'.");
    }

    private static (string hash, string salt) GenerarPasswordHash(string password)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256();
        var saltBytes = hmac.Key;
        var hashBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        return (
            Convert.ToBase64String(hashBytes),
            Convert.ToBase64String(saltBytes)
        );
    }
    public async Task<int> ObtenerIdInternoAsync(
    Guid usuarioGuid,
    CancellationToken cancellationToken = default)
    {
        var model = await _usuarioDataService
            .ObtenerPorGuidAsync(usuarioGuid, cancellationToken);

        if (model is null)
            throw new NotFoundException($"No se encontró el usuario con GUID '{usuarioGuid}'.");

        return model.IdUsuario;
    }
}