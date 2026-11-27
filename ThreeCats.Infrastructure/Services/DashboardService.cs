using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Dashboard;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Domain.Enums;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DashboardService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DashboardDto> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var nowUtc = DateTime.UtcNow;

        var todayStartUtc = DateTime.Today.ToUniversalTime();
        var tomorrowStartUtc = DateTime.Today
            .AddDays(1)
            .ToUniversalTime();

        var todayAppointmentsCount = await context.Appointments
            .CountAsync(
                appointment =>
                    appointment.StartAt >= todayStartUtc
                    && appointment.StartAt < tomorrowStartUtc
                    && appointment.Status != AppointmentStatus.Cancelled,
                cancellationToken);

        var clientsCount = await context.Clients
            .CountAsync(cancellationToken);

        var petsCount = await context.Pets
            .CountAsync(cancellationToken);

        var doctorsCount = await context.Doctors
            .CountAsync(doctor => doctor.IsActive, cancellationToken);

        var upcomingAppointments = await (
            from appointment in context.Appointments

            join pet in context.Pets
                on appointment.PetId equals pet.Id

            join client in context.Clients
                on appointment.ClientId equals client.Id

            join clientUser in context.Users
                on client.UserId equals clientUser.Id

            join doctor in context.Doctors
                on appointment.DoctorId equals doctor.Id

            join doctorUser in context.Users
                on doctor.UserId equals doctorUser.Id

            where appointment.StartAt >= nowUtc
                && appointment.Status != AppointmentStatus.Cancelled

            orderby appointment.StartAt

            select new DashboardAppointmentDto(
                appointment.Id,
                appointment.StartAt,
                pet.Name,
                pet.Species,
                clientUser.LastName + " " +
                    clientUser.FirstName + " " +
                    (clientUser.MiddleName ?? ""),
                doctorUser.LastName + " " +
                    doctorUser.FirstName + " " +
                    (doctorUser.MiddleName ?? ""),
                appointment.Status
            )
        )
        .Take(4)
        .ToListAsync(cancellationToken);

        return new DashboardDto(
            todayAppointmentsCount,
            clientsCount,
            petsCount,
            doctorsCount,
            upcomingAppointments
        );
    }
}
