namespace Microservicio.Pooking.Auth.DataManagement.Models;

/// <summary>
/// Modelo de filtros y parámetros de paginación para búsquedas de usuarios.
/// Lo recibe el servicio de datos desde la capa de negocio.
/// </summary>
public class UsuarioFiltroDataModel
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
    /// Filtro por nombre de rol (ej. "ADMINISTRADOR").
    /// </summary>
    public string? NombreRol { get; set; }

    /// <summary>
    /// Número de página base 1.
    /// </summary>
    public int PaginaActual { get; set; } = 1;

    /// <summary>
    /// Registros por página.
    /// </summary>
    public int TamanioPagina { get; set; } = 10;
}