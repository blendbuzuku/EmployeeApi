using EmployeeApi.Repositories;
using EmployeeApi.Repositories.Interfaces;
using EmployeeApi.Services;
using EmployeeApi.Services.Interfaces;

namespace EmployeeApi.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeAttachmentRepository, EmployeeAttachmentRepository>();
        services.AddScoped<IEmployeeProfilePictureRepository, EmployeeProfilePictureRepository>();

        // Services
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }
}