using ThreeCats.Application.DTOs.Doctors;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Application.Interfaces.Services;

public interface IDoctorManagementService
{
    Task<PagedResult<DoctorDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DoctorDto>> GetAllForSelectionAsync(
        CancellationToken cancellationToken = default);

    Task<DoctorDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateDoctorDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateDoctorDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
