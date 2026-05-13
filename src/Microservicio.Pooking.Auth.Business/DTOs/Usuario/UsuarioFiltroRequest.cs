namespace Microservicio.Pooking.Auth.Business.DTOs.Usuario;

/// <summary>
/// DTO de parámetros de búsqueda y paginación para listados de usuarios.
/// Lo recibe la API desde query parameters y lo pasa al servicio de negocio.
/// </summary>
public class UsuarioFiltroRequest
{
    /// <summary>
    /// Búsqueda parcial por username o correo.
    /// </summary>
    public string? Termino { get; set; }

    /// <summary>
    /// Filtro por estado: ACT o INA.
    /// </summary>
    public string? EstadoUsuario { get; set; }

    /// <summary>
    /// Filtro por nombre de rol.
    /// </summary>
    public string? NombreRol { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}