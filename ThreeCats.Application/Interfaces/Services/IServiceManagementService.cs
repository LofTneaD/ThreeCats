using ThreeCats.Application.DTOs.Services;

namespace ThreeCats.Application.Interfaces.Services;

public interface IServiceManagementService
{
    Task<IReadOnlyList<ServiceDto>> GetAllAsync(
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
