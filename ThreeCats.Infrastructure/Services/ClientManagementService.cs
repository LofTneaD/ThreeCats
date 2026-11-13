using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Clients;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;

namespace ThreeCats.Infrastructure.Services;

public class ClientManagementService : IClientManagementService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ClientManagementService(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<ClientDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Clients
            .AsNoTracking()
            .OrderBy(client => client.Id)
            .Select(client => new ClientDto(
                client.Id,
                client.UserId,
                client.Address,
                client.Notes,
                client.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<ClientDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Clients
            .AsNoTracking()
            .Where(client => client.Id == id)
            .Select(client => new ClientDto(
                client.Id,
                client.UserId,
                client.Address,
                client.Notes,
                client.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        CreateClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var client = new Client
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Address = dto.Address,
            Notes = dto.Notes,
            IsActive = dto.IsActive
        };

        context.Clients.Add(client);

        await context.SaveChangesAsync(cancellationToken);

        return client.Id;
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
