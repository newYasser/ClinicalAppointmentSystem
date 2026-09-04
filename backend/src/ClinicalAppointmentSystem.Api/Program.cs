using ClinicalAppointmentSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("ClinicDb")
    ?? throw new InvalidOperationException(
        "Connection string 'ClinicDb' is not configured. Set it in user-secrets or appsettings."),
    builder.Configuration["Clinic:TimeZone"]
    ?? throw new InvalidOperationException("'Clinic:TimeZone' is not configured."));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
