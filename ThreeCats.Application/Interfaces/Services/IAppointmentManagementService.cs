using ThreeCats.Application.DTOs.Appointments;

namespace ThreeCats.Application.Interfaces.Appointments;

public interface IAppointmentManagementService
{
    Task<IReadOnlyList<AppointmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<AppointmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateAppointmentDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateAppointmentDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
