namespace ThreeCats.Application.DTOs.Clients;

public sealed record CreateClientDto(
    Guid UserId,
    string Address,
    string Notes,
    bool IsActive);
