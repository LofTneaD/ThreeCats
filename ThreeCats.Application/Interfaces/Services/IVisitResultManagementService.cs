using ThreeCats.Application.DTOs.VisitResults;

namespace ThreeCats.Application.Interfaces.VisitResults;

public interface IVisitResultManagementService
{
    Task<IReadOnlyList<VisitResultDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<VisitResultDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateVisitResultDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateVisitResultDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
