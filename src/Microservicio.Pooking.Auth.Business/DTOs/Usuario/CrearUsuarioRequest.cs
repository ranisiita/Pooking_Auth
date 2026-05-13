namespace Microservicio.Pooking.Auth.Business.DTOs.Usuario;

public class CrearUsuarioRequest
{
    // -------------------------------------------------------------------------
    // Datos de usuario
    // -------------------------------------------------------------------------
    public string Username { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NombreRol { get; set; } = string.Empty;
    public string CreadoPorUsuario { get; set; } = string.Empty;


}