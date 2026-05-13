using Microservicio.Pooking.Auth.Api.Extensions;
using Microservicio.Pooking.Auth.Api.Middleware;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

// -------------------------------------------------------------------------
// Servicios base
// -------------------------------------------------------------------------
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.ProducesAttribute("application/json"));
});

// -------------------------------------------------------------------------
// Configuraciones transversales
// -------------------------------------------------------------------------
builder.Services.AddCustomApiVersioning();
builder.Services.AddPookingCors(builder.Configuration);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomSwagger();
builder.Services.AddAuthorization();

// -------------------------------------------------------------------------
// Módulo Auth
// -------------------------------------------------------------------------
builder.Services.AddAuthServices(builder.Configuration);  // ← faltaba esto

// -------------------------------------------------------------------------
// Pipeline HTTP
// -------------------------------------------------------------------------
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pooking Auth API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();