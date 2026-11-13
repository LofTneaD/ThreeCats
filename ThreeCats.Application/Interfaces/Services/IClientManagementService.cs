using ThreeCats.Application.DTOs.Clients;

namespace ThreeCats.Application.Interfaces.Services;

public interface IClientManagementService
{
    Task<IReadOnlyList<ClientDto>> GetAllAsync(
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
