namespace ThreeCats.Application.DTOs.Clients;

public sealed record UpdateClientDto(
    Guid Id,
    string LastName,
    string FirstName,
    string? MiddleName,
    string PhoneNumber,
    string? Email,
    string Address,
    string Notes,
    bool IsActive
);
