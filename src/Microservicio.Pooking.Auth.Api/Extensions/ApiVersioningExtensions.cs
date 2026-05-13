using Asp.Versioning;

namespace Microservicio.Pooking.Auth.Api.Extensions;

/// <summary>
/// Configura el versionamiento de la API por URL (/api/v1/...).
/// </summary>
public static class ApiVersioningExtensions
{
    public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = false;
        });

        return services;
    }
}
