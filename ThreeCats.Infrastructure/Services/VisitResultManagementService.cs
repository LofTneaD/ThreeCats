using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.VisitResults;
using ThreeCats.Application.Interfaces.VisitResults;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public class VisitResultManagementService
    : IVisitResultManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public VisitResultManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<VisitResultDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.VisitResults
            .AsNoTracking()
            .OrderByDescending(result => result.PerformedAt)
            .Select(result => new VisitResultDto(
                result.Id,
                result.VisitId,
                result.Name,
                result.Result,
                result.ReferenceRange,
                result.PerformedAt,
                result.FileUrl))
            .ToListAsync(cancellationToken);
    }

    public async Task<VisitResultDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.VisitResults
            .AsNoTracking()
            .Where(result => result.Id == id)
            .Select(result => new VisitResultDto(
                result.Id,
                result.VisitId,
                result.Name,
                result.Result,
                result.ReferenceRange,
                result.PerformedAt,
                result.FileUrl))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateVisitResultDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        // Проверяем существование посещения.
        var visitExists = await context.Visits
            .AnyAsync(
                visit => visit.Id == dto.VisitId,
                cancellationToken);

        if (!visitExists)
            throw new ArgumentException(
                "Указанное посещение не существует.");

        var visitResult = new VisitResult
        {
            Id = Guid.NewGuid(),
            VisitId = dto.VisitId,
            Name = dto.Name,
            Result = dto.Result,
            ReferenceRange = dto.ReferenceRange,
            PerformedAt = dto.PerformedAt,
            FileUrl = dto.FileUrl
        };

        context.VisitResults.Add(visitResult);

        await context.SaveChangesAsync(cancellationToken);

        return visitResult.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateVisitResultDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var visitResult = await context.VisitResults
            .FirstOrDefaultAsync(
                result => result.Id == dto.Id,
                cancellationToken);

        if (visitResult is null)
            return false;

        visitResult.Name = dto.Name;
        visitResult.Result = dto.Result;
        visitResult.ReferenceRange = dto.ReferenceRange;
        visitResult.PerformedAt = dto.PerformedAt;
        visitResult.FileUrl = dto.FileUrl;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var visitResult = await context.VisitResults
            .FirstOrDefaultAsync(
                result => result.Id == id,
                cancellationToken);

        if (visitResult is null)
            return false;

        context.VisitResults.Remove(visitResult);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
