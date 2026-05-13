using Microservicio.Pooking.Auth.DataAccess.Context;
using Microservicio.Pooking.Auth.Business.Interfaces;
using Microservicio.Pooking.Auth.Business.Services;
using Microservicio.Pooking.Auth.DataManagement.Interfaces;
using Microservicio.Pooking.Auth.DataManagement.Services;
using Microsoft.EntityFrameworkCore;


namespace Microservicio.Pooking.Auth.Api.Extensions;

/// <summary>
/// Registra todos los servicios del módulo de Usuarios (capas 1, 2 y 3).
/// Se llama desde Program.cs con builder.Services.AddUsuariosModule(configuration).
/// Cada módulo del equipo tiene su propia extension de este tipo.
/// </summary>
public static class AuthServiceExtensions
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:Default.");

        // DbContext
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(connectionString));

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // DataManagement
        services.AddScoped<IUsuarioDataService, UsuarioDataService>();
        services.AddScoped<IRolDataService, RolDataService>();

        // Business
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IRolService, RolService>();

        return services;
    }
}
