using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.DoctorSchedules;
using ThreeCats.Application.Interfaces.DoctorSchedules;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public class DoctorScheduleManagementService
    : IDoctorScheduleManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DoctorScheduleManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<DoctorScheduleDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.DoctorSchedules
            .AsNoTracking()
            .OrderBy(schedule => schedule.DoctorId)
            .ThenBy(schedule => schedule.DayOfWeek)
            .ThenBy(schedule => schedule.StartTime)
            .Select(schedule => new DoctorScheduleDto(
                schedule.Id,
                schedule.DoctorId,
                schedule.DayOfWeek,
                schedule.StartTime,
                schedule.EndTime,
                schedule.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<DoctorScheduleDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.DoctorSchedules
            .AsNoTracking()
            .Where(schedule => schedule.Id == id)
            .Select(schedule => new DoctorScheduleDto(
                schedule.Id,
                schedule.DoctorId,
                schedule.DayOfWeek,
                schedule.StartTime,
                schedule.EndTime,
                schedule.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateDoctorScheduleDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var schedule = new DoctorSchedule
        {
            Id = Guid.NewGuid(),
            DoctorId = dto.DoctorId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsActive = dto.IsActive
        };

        context.DoctorSchedules.Add(schedule);

        await context.SaveChangesAsync(cancellationToken);

        return schedule.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateDoctorScheduleDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var schedule = await context.DoctorSchedules
            .FirstOrDefaultAsync(
                schedule => schedule.Id == dto.Id,
                cancellationToken);

        if (schedule is null)
            return false;

        schedule.DayOfWeek = dto.DayOfWeek;
        schedule.StartTime = dto.StartTime;
        schedule.EndTime = dto.EndTime;
        schedule.IsActive = dto.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var schedule = await context.DoctorSchedules
            .FirstOrDefaultAsync(
                schedule => schedule.Id == id,
                cancellationToken);

        if (schedule is null)
            return false;

        context.DoctorSchedules.Remove(schedule);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
