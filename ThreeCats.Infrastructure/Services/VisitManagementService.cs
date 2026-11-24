using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Visits;
using ThreeCats.Application.Interfaces.Visits;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Infrastructure.Services;

public class VisitManagementService : IVisitManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public VisitManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<PagedResult<VisitDto>> GetAllAsync(
    PagedRequest request,
    CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = context.Visits
            .AsNoTracking()
            .OrderByDescending(visit => visit.CreatedAt)
            .ThenBy(visit => visit.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Skip)
            .Take(request.NormalizedPageSize)
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

        return new PagedResult<VisitDto>(
            items,
            request.NormalizedPageNumber,
            request.NormalizedPageSize,
            totalCount);
    }

    public async Task<IReadOnlyList<VisitDto>> GetAllForSelectionAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Visits
            .AsNoTracking()
            .OrderByDescending(visit => visit.CreatedAt)
            .ThenBy(visit => visit.Id)
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

        var appointment = await context.Appointments
            .FirstOrDefaultAsync(
                appointment => appointment.Id == dto.AppointmentId,
                cancellationToken);

        if (appointment is null)
            throw new ArgumentException(
                "Указанная запись на приём не существует.");

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
            AppointmentId = appointment.Id,
            PetId = appointment.PetId,
            DoctorId = appointment.DoctorId,
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

    public async Task<IReadOnlyList<VisitDto>> GetForVaccinationSelectionAsync(
        Guid petId,
        Guid? currentVaccinationId = null,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = context.Visits
            .AsNoTracking()
            .Where(visit => visit.PetId == petId &&
                !context.Vaccinations.Any(vaccination =>
                    vaccination.VisitId == visit.Id &&
                    (currentVaccinationId == null ||
                     vaccination.Id != currentVaccinationId)));

        return await query
            .OrderByDescending(visit => visit.CreatedAt)
            .ThenBy(visit => visit.Id)
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
}
