using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ThreeCats.Application.Constants;
using ThreeCats.Application.DTOs.Users;
using ThreeCats.Application.Interfaces.Users;
using ThreeCats.Application.Models.Pagination;
using ThreeCats.Application.Validation;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Identity;

public class UserManagementService : IUserManagementService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public UserManagementService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var _userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var _roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var query = _userManager.Users
            .AsNoTracking()
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .ThenBy(user => user.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip(request.Skip)
            .Take(request.NormalizedPageSize)
            .ToListAsync(cancellationToken);

        var userIds = users
            .Select(user => user.Id)
            .ToHashSet();

        var doctorUserIds = await _context.Doctors
            .AsNoTracking()
            .Where(doctor => userIds.Contains(doctor.UserId))
            .Select(doctor => doctor.UserId)
            .ToHashSetAsync(cancellationToken);

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var role = doctorUserIds.Contains(user.Id)
                       || roles.Contains(UserRoles.Doctor)
                ? UserRoles.Doctor
                : roles.FirstOrDefault() ?? string.Empty;

            result.Add(new UserDto(
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                role,
                user.LastName,
                user.FirstName,
                user.MiddleName));
        }

        return new PagedResult<UserDto>(
            result,
            request.NormalizedPageNumber,
            request.NormalizedPageSize,
            totalCount);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllForSelectionAsync(
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var _userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var _roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .ThenBy(user => user.Id)
            .ToListAsync(cancellationToken);

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserDto(
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                roles.FirstOrDefault() ?? string.Empty,
                user.LastName,
                user.FirstName,
                user.MiddleName));
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var _userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var _roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);              

        var hasDoctorProfile = await _context.Doctors
            .AnyAsync(
                doctor => doctor.UserId == user.Id,
                cancellationToken);

        var role = hasDoctorProfile || roles.Contains(UserRoles.Doctor)
            ? UserRoles.Doctor
            : roles.FirstOrDefault() ?? string.Empty;

        return new UserDto(
            user.Id,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            role,
            user.LastName,
            user.FirstName,
            user.MiddleName);
    }

    public async Task<Guid> CreateAsync(
        CreateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var _userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var _roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        PhoneNumberValidator.Validate(dto.PhoneNumber);

        var roleExists = await _roleManager.RoleExistsAsync(dto.Role);

        if (!roleExists)
            throw new InvalidOperationException(
                $"Роль '{dto.Role}' не найдена.");

        if (dto.Role == UserRoles.Doctor)
            throw new InvalidOperationException(
                "Для создания врача используйте раздел «Врачи».");

        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        var user = new ApplicationUser
        {
            UserName = PhoneNumberHelper.NormalizeForUserName(dto.PhoneNumber),
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ",
                    createResult.Errors.Select(error => error.Description)));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ",
                    roleResult.Errors.Select(error => error.Description)));
        }

        if (dto.Role == UserRoles.Client)
        {
            await EnsureClientProfileAsync(
                _context,
                user.Id,
                cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        return user.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var _userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var _roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        PhoneNumberValidator.Validate(dto.PhoneNumber);

        var user = await _userManager.FindByIdAsync(dto.Id.ToString());

        if (user is null)
            return false;

        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        var currentRoles = await _userManager.GetRolesAsync(user);

        var hasDoctorProfile = await _context.Doctors
            .AnyAsync(
                doctor => doctor.UserId == user.Id,
                cancellationToken);

        if (hasDoctorProfile && !currentRoles.Contains(UserRoles.Doctor))
        {
            var restoreRoleResult = await _userManager.AddToRoleAsync(
                user,
                UserRoles.Doctor);

            if (!restoreRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ",
                        restoreRoleResult.Errors.Select(e => e.Description)));
            }

            currentRoles = await _userManager.GetRolesAsync(user);
        }

        if ((hasDoctorProfile || currentRoles.Contains(UserRoles.Doctor))
            && dto.Role != UserRoles.Doctor)
        {
            throw new InvalidOperationException(
                "Роль врача нельзя изменить через управление пользователями. " +
                "Используйте раздел «Врачи».");
        }

        if (dto.Role == UserRoles.Doctor
            && !currentRoles.Contains(UserRoles.Doctor))
        {
            throw new InvalidOperationException(
                "Для назначения роли врача используйте раздел «Врачи».");
        }

        var roleExists = await _roleManager.RoleExistsAsync(dto.Role);

        if (!roleExists)
            throw new InvalidOperationException(
                $"Роль '{dto.Role}' не найдена.");

        user.LastName = dto.LastName;
        user.FirstName = dto.FirstName;
        user.MiddleName = dto.MiddleName;

        user.UserName = PhoneNumberHelper.NormalizeForUserName(dto.PhoneNumber);
        user.PhoneNumber = dto.PhoneNumber;

        user.Email = dto.Email;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ",
                    updateResult.Errors.Select(error => error.Description)));
        }

        if (!currentRoles.Contains(dto.Role))
        {
            var addResult = await _userManager.AddToRoleAsync(user, dto.Role);

            if (!addResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ",
                        addResult.Errors.Select(error => error.Description)));
            }
        }

        if (dto.Role == UserRoles.Client)
        {
            await EnsureClientProfileAsync(
                _context,
                user.Id,
                cancellationToken);
        }

        var rolesToRemove = currentRoles
            .Where(role => role != dto.Role)
            .ToArray();

        if (rolesToRemove.Length > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(
                user,
                rolesToRemove);

            if (!removeResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join("; ",
                        removeResult.Errors.Select(error => error.Description)));
            }
        }

        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var _userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var _roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var hasDoctorProfile = await _context.Doctors
            .AnyAsync(
                doctor => doctor.UserId == id,
                cancellationToken);

        if (hasDoctorProfile)
        {
            throw new InvalidOperationException(
                "Нельзя удалить учётную запись врача, пока существует его профиль. " +
                "Сначала удалите профиль в разделе «Врачи».");
        }

        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return false;

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ",
                    result.Errors.Select(error => error.Description)));
        }

        return true;
    }
    private async Task EnsureClientProfileAsync(
        ApplicationDbContext context,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profileExists = await context.Clients
            .AnyAsync(
                client => client.UserId == userId,
                cancellationToken);

        if (profileExists)
            return;

        context.Clients.Add(new Client
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Address = string.Empty,
            Notes = string.Empty,
            IsActive = true
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
