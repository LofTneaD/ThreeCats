using ThreeCats.Application.DTOs.Doctors;

namespace ThreeCats.Application.Interfaces.Services;

public interface IDoctorManagementService
{
    Task<IReadOnlyList<DoctorDto>> GetAllAsync(
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
