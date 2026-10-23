using ThreeCats.Web.Application.Interfaces;
using ThreeCats.Web.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ThreeCats.Web.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddAplicationService(this IServiceCollection services)
        {
            services.AddScoped<IClinicInfoService, ClinicInfoService>();

            return services;
        }
    }
}
