using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Appointments;
using ThreeCats.Application.Interfaces.Appointments;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public class AppointmentManagementService
    : IAppointmentManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public AppointmentManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<AppointmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Appointments
            .AsNoTracking()
            .OrderByDescending(appointment => appointment.StartAt)
            .Select(appointment => new AppointmentDto(
                appointment.Id,
                appointment.ClientId,
                appointment.PetId,
                appointment.DoctorId,
                appointment.ServiceId,
                appointment.CreatedByUserId,
                appointment.StartAt,
                appointment.EndAt,
                appointment.CreatedAt,
                appointment.Status,
                appointment.Comment))
            .ToListAsync(cancellationToken);
    }

    public async Task<AppointmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.Id == id)
            .Select(appointment => new AppointmentDto(
                appointment.Id,
                appointment.ClientId,
                appointment.PetId,
                appointment.DoctorId,
                appointment.ServiceId,
                appointment.CreatedByUserId,
                appointment.StartAt,
                appointment.EndAt,
                appointment.CreatedAt,
                appointment.Status,
                appointment.Comment))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateAppointmentDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.EndAt <= dto.StartAt)
            throw new ArgumentException(
                "Время окончания приёма должно быть позже времени начала.");

        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var petExists = await context.Pets
            .AnyAsync(
                pet => pet.Id == dto.PetId
                    && pet.ClientId == dto.ClientId,
                cancellationToken);

        if (!petExists)
            throw new ArgumentException(
                "Указанный питомец не принадлежит данному клиенту.");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            ClientId = dto.ClientId,
            PetId = dto.PetId,
            DoctorId = dto.DoctorId,
            ServiceId = dto.ServiceId,
            CreatedByUserId = dto.CreatedByUserId,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            CreatedAt = DateTime.UtcNow,
            Status = dto.Status,
            Comment = dto.Comment
        };

        context.Appointments.Add(appointment);

        await context.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateAppointmentDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.EndAt <= dto.StartAt)
            throw new ArgumentException(
                "Время окончания приёма должно быть позже времени начала.");

        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var appointment = await context.Appointments
            .FirstOrDefaultAsync(
                appointment => appointment.Id == dto.Id,
                cancellationToken);

        if (appointment is null)
            return false;

        var petExists = await context.Pets
            .AnyAsync(
                pet => pet.Id == dto.PetId
                    && pet.ClientId == dto.ClientId,
                cancellationToken);

        if (!petExists)
            throw new ArgumentException(
                "Указанный питомец не принадлежит данному клиенту.");

        appointment.ClientId = dto.ClientId;
        appointment.PetId = dto.PetId;
        appointment.DoctorId = dto.DoctorId;
        appointment.ServiceId = dto.ServiceId;
        appointment.StartAt = dto.StartAt;
        appointment.EndAt = dto.EndAt;
        appointment.Status = dto.Status;
        appointment.Comment = dto.Comment;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var appointment = await context.Appointments
            .Include(appointment => appointment.Visit)
            .FirstOrDefaultAsync(
                appointment => appointment.Id == id,
                cancellationToken);

        if (appointment is null)
            return false;

        if (appointment.Visit is not null)
            return false;

        context.Appointments.Remove(appointment);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
