using ClinicalAppointmentSystem.Application.Specialties;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalAppointmentSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISpecialtyService, SpecialtyService>();

        return services;
    }
}
