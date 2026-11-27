using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Vaccinations;
using ThreeCats.Application.Interfaces.Vaccinations;
using ThreeCats.Application.Models.Pagination;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public sealed class VaccinationManagementService(
    IDbContextFactory<ApplicationDbContext> contextFactory)
    : IVaccinationManagementService
{
    public async Task<PagedResult<VaccinationDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = context.Vaccinations
            .AsNoTracking()
            .OrderByDescending(v => v.VaccinatedAt)
            .ThenByDescending(v => v.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Skip)
            .Take(request.NormalizedPageSize)
            .Select(v => new VaccinationDto(
                v.Id,
                v.PetId,
                v.VisitId,
                v.VaccineName,
                v.BatchNumber,
                v.VaccinatedAt,
                v.NextDueAt,
                v.Notes))
            .ToListAsync(cancellationToken);

        return new PagedResult<VaccinationDto>(
            items,
            request.NormalizedPageNumber,
            request.NormalizedPageSize,
            totalCount);
    }

    public async Task<VaccinationDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Vaccinations
            .AsNoTracking()
            .Where(v => v.Id == id)
            .Select(v => new VaccinationDto(
                v.Id,
                v.PetId,
                v.VisitId,
                v.VaccineName,
                v.BatchNumber,
                v.VaccinatedAt,
                v.NextDueAt,
                v.Notes))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateVaccinationDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateDates(dto.VaccinatedAt, dto.NextDueAt);

        await using var context =
            await contextFactory.CreateDbContextAsync(cancellationToken);

        var petExists = await context.Pets
            .AnyAsync(p => p.Id == dto.PetId, cancellationToken);

        if (!petExists)
            throw new ArgumentException("Указанное животное не найдено.");

        if (dto.VisitId.HasValue)
        {
            await ValidateVisitBelongsToPetAsync(
                context,
                dto.VisitId.Value,
                dto.PetId,
                cancellationToken);
        }

        var vaccination = new Vaccination
        {
            Id = Guid.NewGuid(),
            PetId = dto.PetId,
            VisitId = dto.VisitId,
            VaccineName = dto.VaccineName,
            BatchNumber = dto.BatchNumber,
            VaccinatedAt = dto.VaccinatedAt,
            NextDueAt = dto.NextDueAt,
            Notes = dto.Notes
        };

        context.Vaccinations.Add(vaccination);
        await context.SaveChangesAsync(cancellationToken);

        return vaccination.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateVaccinationDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateDates(dto.VaccinatedAt, dto.NextDueAt);

        await using var context =
            await contextFactory.CreateDbContextAsync(cancellationToken);

        var vaccination = await context.Vaccinations
            .FirstOrDefaultAsync(v => v.Id == dto.Id, cancellationToken);

        if (vaccination is null)
            return false;

        if (dto.VisitId.HasValue)
        {
            await ValidateVisitBelongsToPetAsync(
                context,
                dto.VisitId.Value,
                vaccination.PetId,
                cancellationToken);
        }

        vaccination.VisitId = dto.VisitId;
        vaccination.VaccineName = dto.VaccineName;
        vaccination.BatchNumber = dto.BatchNumber;
        vaccination.VaccinatedAt = dto.VaccinatedAt;
        vaccination.NextDueAt = dto.NextDueAt;
        vaccination.Notes = dto.Notes;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await contextFactory.CreateDbContextAsync(cancellationToken);

        var vaccination = await context.Vaccinations
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (vaccination is null)
            return false;

        context.Vaccinations.Remove(vaccination);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void ValidateDates(
        DateOnly vaccinatedAt,
        DateOnly? nextDueAt)
    {
        if (nextDueAt.HasValue && nextDueAt.Value < vaccinatedAt)
        {
            throw new ArgumentException(
                "Дата следующей вакцинации не может быть раньше даты текущей.");
        }
    }
    private static async Task ValidateVisitBelongsToPetAsync(
    ApplicationDbContext context,
    Guid visitId,
    Guid petId,
    CancellationToken cancellationToken)
    {
        var visitPetId = await context.Visits
            .Where(visit => visit.Id == visitId)
            .Select(visit => (Guid?)visit.PetId)
            .FirstOrDefaultAsync(cancellationToken);

        if (visitPetId is null)
            throw new ArgumentException("Указанный визит не найден.");

        if (visitPetId.Value != petId)
        {
            throw new ArgumentException(
                "Указанный визит не относится к выбранному животному.");
        }
    }
}
