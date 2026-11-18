using ThreeCats.Application.DTOs.Services;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Application.Interfaces.Services;

public interface IServiceManagementService
{
    Task<PagedResult<ServiceDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServiceDto>> GetAllForSelectionAsync(
        CancellationToken cancellationToken = default);

    Task<ServiceDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Guid> CreateAsync(
        CreateServiceDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateServiceDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
