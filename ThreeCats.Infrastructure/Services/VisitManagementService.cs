using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Visits;
using ThreeCats.Application.Interfaces.Visits;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public class VisitManagementService : IVisitManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public VisitManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<VisitDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Visits
            .AsNoTracking()
            .OrderByDescending(visit => visit.CreatedAt)
            .Select(visit => new VisitDto(
                visit.Id,
                visit.AppointmentId,
                visit.DoctorId,
                visit.Complaints,
                visit.Diagnosis,
                visit.Treatment,
                visit.Recommendations,
                visit.Notes,
                visit.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<VisitDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Visits
            .AsNoTracking()
            .Where(visit => visit.Id == id)
            .Select(visit => new VisitDto(
                visit.Id,
                visit.AppointmentId,
                visit.DoctorId,
                visit.Complaints,
                visit.Diagnosis,
                visit.Treatment,
                visit.Recommendations,
                visit.Notes,
                visit.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateVisitDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        // Проверяем существование записи на приём.
        var appointmentExists = await context.Appointments
            .AnyAsync(
                appointment => appointment.Id == dto.AppointmentId,
                cancellationToken);

        if (!appointmentExists)
            throw new ArgumentException(
                "Указанная запись на приём не существует.");

        // Проверяем, что для записи ещё не создано посещение.
        var visitExists = await context.Visits
            .AnyAsync(
                visit => visit.AppointmentId == dto.AppointmentId,
                cancellationToken);

        if (visitExists)
            throw new InvalidOperationException(
                "Для данной записи на приём уже существует посещение.");

        var visit = new Visit
        {
            Id = Guid.NewGuid(),
            AppointmentId = dto.AppointmentId,
            DoctorId = dto.DoctorId,
            Complaints = dto.Complaints,
            Diagnosis = dto.Diagnosis,
            Treatment = dto.Treatment,
            Recommendations = dto.Recommendations,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        context.Visits.Add(visit);

        await context.SaveChangesAsync(cancellationToken);

        return visit.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateVisitDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var visit = await context.Visits
            .FirstOrDefaultAsync(
                visit => visit.Id == dto.Id,
                cancellationToken);

        if (visit is null)
            return false;

        visit.DoctorId = dto.DoctorId;
        visit.Complaints = dto.Complaints;
        visit.Diagnosis = dto.Diagnosis;
        visit.Treatment = dto.Treatment;
        visit.Recommendations = dto.Recommendations;
        visit.Notes = dto.Notes;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var visit = await context.Visits
            .FirstOrDefaultAsync(
                visit => visit.Id == id,
                cancellationToken);

        if (visit is null)
            return false;

        // Не удаляем посещение, если существуют результаты обследований.
        var hasResults = await context.VisitResults
            .AnyAsync(
                result => result.VisitId == id,
                cancellationToken);

        if (hasResults)
            return false;

        context.Visits.Remove(visit);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
