using ThreeCats.Application.DTOs.Clients;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Application.Interfaces.Services;

public interface IClientManagementService
{
    Task<PagedResult<ClientDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClientDto>> GetAllForSelectionAsync(
        CancellationToken cancellationToken = default);

    Task<ClientDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateClientDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateClientDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
