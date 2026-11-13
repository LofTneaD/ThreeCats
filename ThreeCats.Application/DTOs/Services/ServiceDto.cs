namespace ThreeCats.Application.DTOs.Services;

public sealed record ServiceDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    bool IsActive);
