namespace ThreeCats.Application.DTOs.Clients;

public sealed record ClientDto(
    Guid Id,
    Guid UserId,
    string LastName,
    string FirstName,
    string? MiddleName,
    string? PhoneNumber,
    string? Email,
    string Address,
    string Notes,
    bool IsActive
);
