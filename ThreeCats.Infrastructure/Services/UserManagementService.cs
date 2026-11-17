using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.Constants;
using ThreeCats.Application.DTOs.Users;
using ThreeCats.Application.Interfaces.Users;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Domain.Entities;

namespace ThreeCats.Infrastructure.Identity;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var doctorUserIds = await context.Doctors
            .AsNoTracking()
            .Select(doctor => doctor.UserId)
            .ToHashSetAsync(cancellationToken);

        var users = await _userManager.Users
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .ToListAsync(cancellationToken);

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

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var hasDoctorProfile = await context.Doctors
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
        var roleExists = await _roleManager.RoleExistsAsync(dto.Role);

        if (!roleExists)
            throw new InvalidOperationException(
                $"Роль '{dto.Role}' не найдена.");

        if (dto.Role == UserRoles.Doctor)
            throw new InvalidOperationException(
                "Для создания врача используйте раздел «Врачи».");

        var user = new ApplicationUser
        {
            UserName = dto.PhoneNumber,
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
            await _userManager.DeleteAsync(user);

            throw new InvalidOperationException(
                string.Join("; ",
                    roleResult.Errors.Select(error => error.Description)));
        }

        if (dto.Role == UserRoles.Client)
        {
            await EnsureClientProfileAsync(user.Id, cancellationToken);
        }

        return user.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(dto.Id.ToString());

        if (user is null)
            return false;

        var currentRoles = await _userManager.GetRolesAsync(user);

        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var hasDoctorProfile = await context.Doctors
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

        user.UserName = dto.PhoneNumber;
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
            await EnsureClientProfileAsync(user.Id, cancellationToken);
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

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var hasDoctorProfile = await context.Doctors
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
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var profileExists = await context.Clients
            .AnyAsync(client => client.UserId == userId, cancellationToken);

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
