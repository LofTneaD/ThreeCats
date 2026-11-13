using ThreeCats.Application.DTOs.Visits;

namespace ThreeCats.Application.Interfaces.Visits;

public interface IVisitManagementService
{
    Task<IReadOnlyList<VisitDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<VisitDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateVisitDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateVisitDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
