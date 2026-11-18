
using ThreeCats.Application.DTOs.Appointments;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Application.Interfaces.Appointments;

public interface IAppointmentManagementService
{
    Task<PagedResult<AppointmentDto>> GetAllAsync(
        PagedRequest request,
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

    Task<IReadOnlyList<AppointmentDto>> GetAllForSelectionAsync(
        CancellationToken cancellationToken = default);
}
