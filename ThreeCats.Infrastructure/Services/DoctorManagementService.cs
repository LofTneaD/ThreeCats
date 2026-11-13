using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Doctors;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public class DoctorManagementService : IDoctorManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DoctorManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<DoctorDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Doctors
            .AsNoTracking()
            .OrderBy(doctor => doctor.Specialization)
            .Select(doctor => new DoctorDto(
                doctor.Id,
                doctor.UserId,
                doctor.Specialization,
                doctor.Description,
                doctor.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<DoctorDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Doctors
            .AsNoTracking()
            .Where(doctor => doctor.Id == id)
            .Select(doctor => new DoctorDto(
                doctor.Id,
                doctor.UserId,
                doctor.Specialization,
                doctor.Description,
                doctor.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateDoctorDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Specialization = dto.Specialization,
            Description = dto.Description,
            IsActive = dto.IsActive
        };

        context.Doctors.Add(doctor);

        await context.SaveChangesAsync(cancellationToken);

        return doctor.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateDoctorDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var doctor = await context.Doctors
            .FirstOrDefaultAsync(
                doctor => doctor.Id == dto.Id,
                cancellationToken);

        if (doctor is null)
            return false;

        doctor.UserId = dto.UserId;
        doctor.Specialization = dto.Specialization;
        doctor.Description = dto.Description;
        doctor.IsActive = dto.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var doctor = await context.Doctors
            .FirstOrDefaultAsync(
                doctor => doctor.Id == id,
                cancellationToken);

        if (doctor is null)
            return false;

        context.Doctors.Remove(doctor);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
