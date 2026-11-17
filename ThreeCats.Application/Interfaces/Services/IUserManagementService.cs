using ThreeCats.Application.DTOs.Users;

namespace ThreeCats.Application.Interfaces.Users;

public interface IUserManagementService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        CreateUserDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateUserDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
