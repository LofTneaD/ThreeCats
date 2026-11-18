using ThreeCats.Application.Models.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.Constants;
using ThreeCats.Application.DTOs.Clients;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Infrastructure.Identity;
using ThreeCats.Application.Validation;

namespace ThreeCats.Infrastructure.Services;

public class ClientManagementService : IClientManagementService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public ClientManagementService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task<PagedResult<ClientDto>> GetAllAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query =
            from client in _context.Clients.AsNoTracking()
            join user in _context.Users.AsNoTracking()
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
            );

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Skip)
            .Take(request.NormalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ClientDto>(
            items,
            request.NormalizedPageNumber,
            request.NormalizedPageSize,
            totalCount);
    }

    public async Task<IReadOnlyList<ClientDto>> GetAllForSelectionAsync(
        CancellationToken cancellationToken = default)
    {
        return await (
            from client in _context.Clients.AsNoTracking()
            join user in _context.Users.AsNoTracking()
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
        return await (
            from client in _context.Clients.AsNoTracking()
            join user in _context.Users.AsNoTracking()
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
        PhoneNumberValidator.Validate(dto.PhoneNumber);

        var roleExists = await _roleManager.RoleExistsAsync(UserRoles.Client);

        if (!roleExists)
            throw new InvalidOperationException("Роль CLIENT не найдена.");

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

        var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Client);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ",
                    roleResult.Errors.Select(error => error.Description)));
        }

        var client = new Client
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Address = dto.Address,
            Notes = dto.Notes,
            IsActive = dto.IsActive
        };

        _context.Clients.Add(client);

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return client.Id;
    }

    public async Task<bool> UpdateAsync(
    UpdateClientDto dto,
    CancellationToken cancellationToken = default)
    {
        PhoneNumberValidator.Validate(dto.PhoneNumber);

        var client = await _context.Clients
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

        client.Address = dto.Address;
        client.Notes = dto.Notes;
        client.IsActive = dto.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(
                client => client.Id == id,
                cancellationToken);

        if (client is null)
            return false;

        _context.Clients.Remove(client);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
