using Microservicio.Pooking.Auth.Api.Models.Common;
using Microservicio.Pooking.Auth.Business.Exceptions;
using BusinessValidationException = Microservicio.Pooking.Auth.Business.Exceptions.ValidationException;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Microservicio.Pooking.Auth.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado: {Mensaje}", ex.Message);
            await EscribirRespuestaAsync(context, ex);
        }
    }

    private static async Task EscribirRespuestaAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var traceId = context.TraceIdentifier;

        var (statusCode, body) = exception switch
        {
            BusinessValidationException ve => (
                (int)HttpStatusCode.BadRequest,
                ApiErrorResponse.Crear(ve.Message, (int)HttpStatusCode.BadRequest, ve.Errors.ToArray(), traceId)),
            NotFoundException ne => (
                (int)HttpStatusCode.NotFound,
                ApiErrorResponse.Crear(ne.Message, (int)HttpStatusCode.NotFound, new[] { ne.Message }, traceId)),
            BusinessException be => (
                (int)HttpStatusCode.Conflict,
                ApiErrorResponse.Crear(be.Message, (int)HttpStatusCode.Conflict, new[] { be.Message }, traceId)),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                ApiErrorResponse.Crear("Error interno del servidor. Por favor, intente nuevamente o contacte a soporte si el problema persiste.", (int)HttpStatusCode.InternalServerError, Array.Empty<string>(), traceId))
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
