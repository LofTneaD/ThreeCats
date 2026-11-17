
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.Constants;
using ThreeCats.Application.DTOs.Clients;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Infrastructure.Identity;

namespace ThreeCats.Infrastructure.Services;

public class ClientManagementService : IClientManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public ClientManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<ClientDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await (
            from client in context.Clients.AsNoTracking()
            join user in context.Users.AsNoTracking()
                on client.UserId equals user.Id
            orderby user.LastName, user.FirstName
            select new ClientDto(
                client.Id,
                client.UserId,
                user.LastName,
                user.FirstName,
                user.MiddleName,
                user.PhoneNumber,
                user.Email,
                client.Address,
                client.Notes,
                client.IsActive
            )
        ).ToListAsync(cancellationToken);
    }

    public async Task<ClientDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await (
            from client in context.Clients.AsNoTracking()
            join user in context.Users.AsNoTracking()
                on client.UserId equals user.Id
            where client.Id == id
            select new ClientDto(
                client.Id,
                client.UserId,
                user.LastName,
                user.FirstName,
                user.MiddleName,
                user.PhoneNumber,
                user.Email,
                client.Address,
                client.Notes,
                client.IsActive
            )
        ).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateClientDto dto,
        CancellationToken cancellationToken = default)
    {
        var roleExists = await _roleManager.RoleExistsAsync(
            UserRoles.Client);

        if (!roleExists)
            throw new InvalidOperationException(
                "Роль CLIENT не найдена.");

        var user = new ApplicationUser
        {
            UserName = dto.PhoneNumber,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName
        };

        var createResult = await _userManager.CreateAsync(
            user,
            dto.Password);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ",
                    createResult.Errors.Select(
                        error => error.Description)));
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            UserRoles.Client);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            throw new InvalidOperationException(
                string.Join("; ",
                    roleResult.Errors.Select(
                        error => error.Description)));
        }

        try
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Address = dto.Address,
                Notes = dto.Notes,
                IsActive = dto.IsActive
            };

            context.Clients.Add(client);

            await context.SaveChangesAsync(cancellationToken);

            return client.Id;
        }
        catch (Exception ex)
        {
            var rollbackResult = await _userManager.DeleteAsync(user);

            var rollbackMessage = rollbackResult.Succeeded
                ? string.Empty
                : " Не удалось удалить созданную учётную запись: "
                  + string.Join("; ",
                      rollbackResult.Errors.Select(
                          error => error.Description));

            throw new InvalidOperationException(
                "Не удалось создать профиль клиента." + rollbackMessage,
                ex);
        }
    }

    public async Task<bool> UpdateAsync(
        UpdateClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var client = await context.Clients
            .FirstOrDefaultAsync(
                client => client.Id == dto.Id,
                cancellationToken);

        if (client is null)
            return false;

        var user = await _userManager.FindByIdAsync(
            client.UserId.ToString());

        if (user is null)
            throw new InvalidOperationException(
                "Учётная запись клиента не найдена.");

        // Телефон используется как UserName.
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
                    userUpdateResult.Errors.Select(
                        error => error.Description)));
        }

        client.Address = dto.Address;
        client.Notes = dto.Notes;
        client.IsActive = dto.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var client = await context.Clients
            .FirstOrDefaultAsync(
                client => client.Id == id,
                cancellationToken);

        if (client is null)
            return false;

        context.Clients.Remove(client);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
