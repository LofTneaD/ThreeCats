using ThreeCats.Application.DTOs.Visits;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Application.Interfaces.Visits;

public interface IVisitManagementService
{
    Task<PagedResult<VisitDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitDto>> GetAllForSelectionAsync(
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
