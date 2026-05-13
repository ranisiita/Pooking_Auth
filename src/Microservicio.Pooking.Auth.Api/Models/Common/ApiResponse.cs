namespace Microservicio.Pooking.Auth.Api.Models.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> Exitoso(T data, string message = "Operación exitosa", int statusCode = 200) =>
        new() { Success = true, StatusCode = statusCode, Message = message, Data = data };

    public static ApiResponse<T> SinContenido(string message = "Sin datos", int statusCode = 204) =>
        new() { Success = true, StatusCode = statusCode, Message = message, Data = default };

    // Alias para compatibilidad con controllers que usan .Ok()
    public static ApiResponse<T> Ok(T data, string message = "Operación exitosa", int statusCode = 200) =>
        Exitoso(data, message, statusCode);
}