using System.Text.Json.Serialization;
using ClinicalAppointmentSystem.Api.Authentication;
using ClinicalAppointmentSystem.Api.ErrorHandling;
using ClinicalAppointmentSystem.Application;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Infrastructure;
using Microsoft.AspNetCore.Mvc;

const string CorsPolicy = "AngularClient";

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<DomainExceptionHandler>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Instance = context.HttpContext.Request.Path,
        };

        problemDetails.Extensions["errorCode"] = ErrorCodes.ValidationFailed;

        return new BadRequestObjectResult(problemDetails);
    };
});

builder.Services.AddCors(options =>
    options.AddPolicy(CorsPolicy, policy => policy
        .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod()));

var jwtSettings = builder.Configuration.ReadJwtSettings();
var googleSettings = builder.Configuration.ReadGoogleSettings();

builder.Services.AddJwtAuthentication(jwtSettings);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("ClinicDb")
    ?? throw new InvalidOperationException(
        "Connection string 'ClinicDb' is not configured. Set it in user-secrets or appsettings."),
    builder.Configuration["Clinic:TimeZone"]
    ?? throw new InvalidOperationException("'Clinic:TimeZone' is not configured."),
    jwtSettings,
    googleSettings);

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
