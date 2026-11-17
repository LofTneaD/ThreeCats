using ThreeCats.Application.DTOs.Dashboard;

namespace ThreeCats.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(
        CancellationToken cancellationToken = default);
}
