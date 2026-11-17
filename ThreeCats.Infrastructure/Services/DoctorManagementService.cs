using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.Constants;
using ThreeCats.Application.DTOs.Doctors;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Infrastructure.Identity;

namespace ThreeCats.Infrastructure.Services;

public class DoctorManagementService : IDoctorManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public DoctorManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<DoctorDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await (
            from doctor in context.Doctors.AsNoTracking()
            join user in context.Users.AsNoTracking()
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
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await (
            from doctor in context.Doctors.AsNoTracking()
            join user in context.Users.AsNoTracking()
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
        var roleExists = await _roleManager.RoleExistsAsync(UserRoles.Doctor);

        if (!roleExists)
            throw new InvalidOperationException("Роль DOCTOR не найдена.");

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

        var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Doctor);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            throw new InvalidOperationException(
                string.Join("; ",
                    roleResult.Errors.Select(error => error.Description)));
        }

        try
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Specialization = dto.Specialization,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            context.Doctors.Add(doctor);

            await context.SaveChangesAsync(cancellationToken);

            return doctor.Id;
        }
        catch (Exception ex)
        {
            var rollbackResult = await _userManager.DeleteAsync(user);

            var rollbackMessage = rollbackResult.Succeeded
                ? string.Empty
                : " Не удалось удалить созданную учётную запись: "
                  + string.Join("; ",
                      rollbackResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                "Не удалось создать профиль врача." + rollbackMessage,
                ex);
        }
    }

    public async Task<bool> UpdateAsync(
        UpdateDoctorDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var doctor = await context.Doctors
            .FirstOrDefaultAsync(
                doctor => doctor.Id == dto.Id,
                cancellationToken);

        if (doctor is null)
            return false;

        var user = await _userManager.FindByIdAsync(doctor.UserId.ToString());

        if (user is null)
            throw new InvalidOperationException(
                "Учётная запись врача не найдена.");

        // Телефон используется как UserName,
        // поэтому обновляем оба поля.
        user.UserName = dto.PhoneNumber;
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

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var doctor = await context.Doctors
            .FirstOrDefaultAsync(
                doctor => doctor.Id == id,
                cancellationToken);

        if (doctor is null)
            return false;

        context.Doctors.Remove(doctor);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
