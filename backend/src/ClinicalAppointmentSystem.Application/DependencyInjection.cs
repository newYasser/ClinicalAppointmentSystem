using ClinicalAppointmentSystem.Application.Dashboard;
using ClinicalAppointmentSystem.Application.Doctors;
using ClinicalAppointmentSystem.Application.Patients;
using ClinicalAppointmentSystem.Application.Specialties;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalAppointmentSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISpecialtyService, SpecialtyService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
