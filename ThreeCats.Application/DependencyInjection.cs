using Microsoft.Extensions.DependencyInjection;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Application.Services;

namespace ThreeCats.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAplicationService(this IServiceCollection services)
    {
        services.AddScoped<IClinicInfoService, ClinicInfoService>();
        return services;
    }
}
