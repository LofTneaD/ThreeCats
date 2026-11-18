using ThreeCats.Application.DTOs.Users;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Application.Interfaces.Users;

public interface IUserManagementService
{
    Task<PagedResult<UserDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserDto>> GetAllForSelectionAsync(
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
