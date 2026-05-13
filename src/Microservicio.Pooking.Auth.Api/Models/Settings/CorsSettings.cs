namespace Microservicio.Pooking.Auth.Api.Models.Settings;

public sealed class CorsSettings
{
    public const string SectionName = "Cors";

    /// <summary>Orígenes permitidos (ej. https://localhost:4200).</summary>
    public string[] AllowedOrigins { get; set; } = [];
}