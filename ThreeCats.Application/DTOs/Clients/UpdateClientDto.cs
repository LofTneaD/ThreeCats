namespace ThreeCats.Application.DTOs.Clients;

public sealed record UpdateClientDto(
    Guid Id,
    string Address,
    string Notes,
    bool IsActive);
