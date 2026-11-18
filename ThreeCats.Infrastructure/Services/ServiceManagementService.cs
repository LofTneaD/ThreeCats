using Microsoft.EntityFrameworkCore;
using ThreeCats.Application.DTOs.Services;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Application.Validation;
using ThreeCats.Application.Models.Pagination;

namespace ThreeCats.Infrastructure.Services
{
    public class ServiceManagementService : IServiceManagementService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ServiceManagementService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<PagedResult<ServiceDto>> GetAllAsync(
            PagedRequest request,
            CancellationToken cancellationToken = default)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            var query = context.Services
                .AsNoTracking()
                .OrderBy(service => service.Name)
                .ThenBy(service => service.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip(request.Skip)
                .Take(request.NormalizedPageSize)
                .Select(service => new ServiceDto(
                    service.Id,
                    service.Name,
                    service.Description,
                    service.Price,
                    service.IsActive))
                .ToListAsync(cancellationToken);

            return new PagedResult<ServiceDto>(
                items,
                request.NormalizedPageNumber,
                request.NormalizedPageSize,
                totalCount);
        }

        public async Task<IReadOnlyList<ServiceDto>> GetAllForSelectionAsync(
            CancellationToken cancellationToken = default)
        {
            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Services
                .AsNoTracking()
                .OrderBy(service => service.Name)
                .ThenBy(service => service.Id)
                .Select(service => new ServiceDto(
                    service.Id,
                    service.Name,
                    service.Description,
                    service.Price,
                    service.IsActive))
                .ToListAsync(cancellationToken);
        }

        public async Task<ServiceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Services
                .AsNoTracking()
                .Where(service => service.Id == id)
                .Select(service => new ServiceDto(
                    service.Id,
                    service.Name,
                    service.Description,
                    service.Price,
                    service.IsActive))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Guid> CreateAsync(
            CreateServiceDto dto,
            CancellationToken cancellationToken = default)
        {
            ServiceValidator.Validate(dto);

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var service = new Service
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                IsActive = dto.IsActive
            };

            context.Services.Add(service);

            await context.SaveChangesAsync(cancellationToken);

            return service.Id;
        }

        public async Task<bool> UpdateAsync(
            UpdateServiceDto dto,
            CancellationToken cancellationToken = default)
        {
            ServiceValidator.Validate(dto);

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var service = await context.Services
                .FirstOrDefaultAsync(service => service.Id == dto.Id, cancellationToken);

            if (service is null)
                return false;

            service.Name = dto.Name;
            service.Description = dto.Description;
            service.Price = dto.Price;
            service.IsActive = dto.IsActive;

            await context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var service = await context.Services.FirstOrDefaultAsync(
                service => service.Id == id,
                cancellationToken);

            if (service is null)
                return false;

            context.Services.Remove(service);

            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
