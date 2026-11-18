using ThreeCats.Application.DTOs.Vaccinations;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Application.Interfaces.Vaccinations;

public interface IVaccinationManagementService
{
    Task<PagedResult<VaccinationDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<VaccinationDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateVaccinationDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateVaccinationDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
