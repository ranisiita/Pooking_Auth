using Microsoft.OpenApi;

namespace Microservicio.Pooking.Auth.Api.Extensions;

/// <summary>
/// Configura Swagger/OpenAPI con soporte para autenticación JWT Bearer.
/// </summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Microservicio Pooking API",
                Version = "v1",
                Description = "API REST v1 — Gestión de Usuarios, Roles y Autenticación"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Ingrese el token JWT con el prefijo Bearer. Ejemplo: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            // En Microsoft.OpenApi 2.x, OpenApiReference fue eliminado.
            // Se usa OpenApiSecuritySchemeReference con delegate que recibe el document.
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });

        return services;
    }
}
