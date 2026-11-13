namespace ThreeCats.Application.DTOs.Clients;

public sealed record ClientDto(
    Guid Id,
    Guid UserId,
    string Address,
    string Notes,
    bool IsActive);
