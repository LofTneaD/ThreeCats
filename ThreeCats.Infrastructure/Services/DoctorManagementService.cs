using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ThreeCats.Application.Constants;
using ThreeCats.Application.DTOs.Doctors;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Application.Models.Pagination;
using ThreeCats.Application.Validation;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Infrastructure.Identity;

namespace ThreeCats.Infrastructure.Services;

public class DoctorManagementService : IDoctorManagementService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DoctorManagementService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }


    public async Task<PagedResult<DoctorDto>> GetAllAsync(
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

        var query =
            from doctor in _context.Doctors.AsNoTracking()
            join user in _context.Users.AsNoTracking()
                on doctor.UserId equals user.Id
            orderby user.LastName, user.FirstName
            select new DoctorDto(
                doctor.Id,
                doctor.UserId,
                user.LastName,
                user.FirstName,
                user.MiddleName,
                user.PhoneNumber,
                user.Email,
                doctor.Specialization,
                doctor.Description,
                doctor.IsActive
            );

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Skip)
            .Take(request.NormalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<DoctorDto>(
            items,
            request.NormalizedPageNumber,
            request.NormalizedPageSize,
            totalCount);
    }

    public async Task<IReadOnlyList<DoctorDto>> GetAllForSelectionAsync(
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var _userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var _roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        return await (
            from doctor in _context.Doctors.AsNoTracking()
            join user in _context.Users.AsNoTracking()
                on doctor.UserId equals user.Id
            orderby user.LastName, user.FirstName
            select new DoctorDto(
                doctor.Id,
                doctor.UserId,
                user.LastName,
                user.FirstName,
                user.MiddleName,
                user.PhoneNumber,
                user.Email,
                doctor.Specialization,
                doctor.Description,
                doctor.IsActive
            )
        ).ToListAsync(cancellationToken);
    }

    public async Task<DoctorDto?> GetByIdAsync(
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

        return await (
            from doctor in _context.Doctors.AsNoTracking()
            join user in _context.Users.AsNoTracking()
                on doctor.UserId equals user.Id
            where doctor.Id == id
            select new DoctorDto(
                doctor.Id,
                doctor.UserId,
                user.LastName,
                user.FirstName,
                user.MiddleName,
                user.PhoneNumber,
                user.Email,
                doctor.Specialization,
                doctor.Description,
                doctor.IsActive
            )
        ).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateDoctorDto dto,
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

        var roleExists = await _roleManager.RoleExistsAsync(UserRoles.Doctor);

        if (!roleExists)
            throw new InvalidOperationException("Роль DOCTOR не найдена.");

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

        var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Doctor);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ",
                    roleResult.Errors.Select(error => error.Description)));
        }

        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Specialization = dto.Specialization,
            Description = dto.Description,
            IsActive = dto.IsActive
        };

        _context.Doctors.Add(doctor);

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return doctor.Id;
    }

    public async Task<bool> UpdateAsync(
        UpdateDoctorDto dto,
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

        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(
                doctor => doctor.Id == dto.Id,
                cancellationToken);

        if (doctor is null)
            return false;

        var user = await _userManager.FindByIdAsync(doctor.UserId.ToString());

        if (user is null)
            throw new InvalidOperationException(
                "Учётная запись врача не найдена.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        user.UserName = PhoneNumberHelper.NormalizeForUserName(dto.PhoneNumber);
        user.PhoneNumber = dto.PhoneNumber;

        user.Email = dto.Email;

        user.LastName = dto.LastName;
        user.FirstName = dto.FirstName;
        user.MiddleName = dto.MiddleName;

        var userUpdateResult = await _userManager.UpdateAsync(user);

        if (!userUpdateResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ",
                    userUpdateResult.Errors.Select(error => error.Description)));
        }

        doctor.Specialization = dto.Specialization;
        doctor.Description = dto.Description;
        doctor.IsActive = dto.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

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

        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(
                doctor => doctor.Id == id,
                cancellationToken);

        if (doctor is null)
            return false;

        _context.Doctors.Remove(doctor);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
