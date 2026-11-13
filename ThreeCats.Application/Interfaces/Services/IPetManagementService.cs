using ThreeCats.Application.DTOs.Pets;

namespace ThreeCats.Application.Interfaces.Pets;

public interface IPetManagementService
{
    Task<IReadOnlyList<PetDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PetDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreatePetDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdatePetDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
