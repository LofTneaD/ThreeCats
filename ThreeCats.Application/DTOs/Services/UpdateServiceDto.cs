namespace ThreeCats.Application.DTOs.Services;

public sealed record UpdateServiceDto(
    Guid Id,
    string Name,
    string Desctiption,
    decimal Price,
    bool IsActive);
