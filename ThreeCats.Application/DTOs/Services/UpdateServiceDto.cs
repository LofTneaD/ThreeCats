namespace ThreeCats.Application.DTOs.Services;

public sealed record UpdateServiceDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    bool IsActive);
