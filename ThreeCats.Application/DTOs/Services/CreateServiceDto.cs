namespace ThreeCats.Application.DTOs.Services;

public sealed record CreateServiceDto(
    string Name,
    string Description,
    decimal Price,
    bool IsActive);
