using ThreeCats.Application.DTOs.DoctorSchedules;

namespace ThreeCats.Application.Interfaces.DoctorSchedules;

public interface IDoctorScheduleManagementService
{
    Task<IReadOnlyList<DoctorScheduleDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DoctorScheduleDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateDoctorScheduleDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateDoctorScheduleDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
