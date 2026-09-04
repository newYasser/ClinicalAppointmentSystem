using ClinicalAppointmentSystem.Api.ErrorHandling;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("ClinicDb")
    ?? throw new InvalidOperationException(
        "Connection string 'ClinicDb' is not configured. Set it in user-secrets or appsettings."),
    builder.Configuration["Clinic:TimeZone"]
    ?? throw new InvalidOperationException("'Clinic:TimeZone' is not configured."));

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
