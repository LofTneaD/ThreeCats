using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Vaccinations;
using ThreeCats.Application.Interfaces.Vaccinations;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public sealed class VaccinationManagementService(
    IDbContextFactory<ApplicationDbContext> contextFactory)
    : IVaccinationManagementService
{
    public async Task<List<VaccinationDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Vaccinations
            .AsNoTracking()
            .OrderByDescending(v => v.VaccinatedAt)
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
            var visitExists = await context.Visits
                .AnyAsync(v => v.Id == dto.VisitId.Value, cancellationToken);

            if (!visitExists)
                throw new ArgumentException("Указанный визит не найден.");
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
            var visitExists = await context.Visits
                .AnyAsync(v => v.Id == dto.VisitId.Value, cancellationToken);

            if (!visitExists)
                throw new ArgumentException("Указанный визит не найден.");
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
}
