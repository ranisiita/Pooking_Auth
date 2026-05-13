namespace Microservicio.Pooking.Auth.Api.Models.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>Si es false, no se registra JWT Bearer ni se exige token en los controladores v1 de este plan.</summary>
    public bool Enabled { get; set; }

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
