namespace ThreeCats.Application.DTOs.Clients;

public sealed record CreateClientDto(
    string LastName,
    string FirstName,
    string? MiddleName,
    string PhoneNumber,
    string? Email,
    string Password,
    string Address,
    string Notes,
    bool IsActive
);
