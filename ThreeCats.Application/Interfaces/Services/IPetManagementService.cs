using ThreeCats.Application.DTOs.Pets;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Application.Interfaces.Pets;

public interface IPetManagementService
{
    Task<PagedResult<PetDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PetDto>> GetAllForSelectionAsync(
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

    Task<IReadOnlyList<PetDto>> SearchForSelectionAsync(
        string? searchTerm,
        int limit = 20,
        CancellationToken cancellationToken = default);
}
