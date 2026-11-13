using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThreeCats.Application.Interfaces.Appointments;
using ThreeCats.Application.Interfaces.DoctorSchedules;
using ThreeCats.Application.Interfaces.Pets;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Application.Interfaces.Vaccinations;
using ThreeCats.Application.Interfaces.VisitResults;
using ThreeCats.Application.Interfaces.Visits;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Infrastructure.Identity;
using ThreeCats.Infrastructure.Services;

namespace ThreeCats.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IServiceManagementService, ServiceManagementService>();
        services.AddScoped<IDoctorManagementService, DoctorManagementService>();
        services.AddScoped<IClientManagementService, ClientManagementService>();
        services.AddScoped<IPetManagementService, PetManagementService>();
        services.AddScoped<IDoctorScheduleManagementService, DoctorScheduleManagementService>();
        services.AddScoped<IAppointmentManagementService, AppointmentManagementService>();
        services.AddScoped<IVisitManagementService, VisitManagementService>();
        services.AddScoped<IVisitResultManagementService, VisitResultManagementService>();
        services.AddScoped<IVaccinationManagementService, VaccinationManagementService>();

        return services;
    }
}
