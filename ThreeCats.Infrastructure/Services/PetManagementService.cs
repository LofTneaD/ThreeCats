using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Pets;
using ThreeCats.Application.Interfaces.Pets;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public class PetManagementService : IPetManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public PetManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<PetDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Pets
            .AsNoTracking()
            .OrderBy(pet => pet.Name)
            .Select(pet => new PetDto(
                pet.Id,
                pet.ClientId,
                pet.Name,
                pet.Species,
                pet.Breed,
                pet.Sex,
                pet.BirthDate,
                pet.Weight,
                pet.Notes))
            .ToListAsync(cancellationToken);
    }

    public async Task<PetDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Pets
            .AsNoTracking()
            .Where(pet => pet.Id == id)
            .Select(pet => new PetDto(
                pet.Id,
                pet.ClientId,
                pet.Name,
                pet.Species,
                pet.Breed,
                pet.Sex,
                pet.BirthDate,
                pet.Weight,
                pet.Notes))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreatePetDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var pet = new Pet
        {
            Id = Guid.NewGuid(),
            ClientId = dto.ClientId,
            Name = dto.Name,
            Species = dto.Species,
            Breed = dto.Breed,
            Sex = dto.Sex,
            BirthDate = dto.BirthDate,
            Weight = dto.Weight,
            Notes = dto.Notes
        };

        context.Pets.Add(pet);

        await context.SaveChangesAsync(cancellationToken);

        return pet.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdatePetDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var pet = await context.Pets
            .FirstOrDefaultAsync(
                pet => pet.Id == dto.Id,
                cancellationToken);

        if (pet is null)
            return false;

        pet.Name = dto.Name;
        pet.Species = dto.Species;
        pet.Breed = dto.Breed;
        pet.Sex = dto.Sex;
        pet.BirthDate = dto.BirthDate;
        pet.Weight = dto.Weight;
        pet.Notes = dto.Notes;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var pet = await context.Pets
            .FirstOrDefaultAsync(
                pet => pet.Id == id,
                cancellationToken);

        if (pet is null)
            return false;

        context.Pets.Remove(pet);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
