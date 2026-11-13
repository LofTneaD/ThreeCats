using ThreeCats.Application.DTOs.Vaccinations;

namespace ThreeCats.Application.Interfaces.Vaccinations;

public interface IVaccinationManagementService
{
    Task<List<VaccinationDto>> GetAllAsync(
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
