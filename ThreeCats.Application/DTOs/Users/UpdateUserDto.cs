namespace ThreeCats.Application.DTOs.Users;

public sealed record UpdateUserDto(
    Guid Id,
    string LastName,
    string FirstName,
    string? MiddleName,
    string PhoneNumber,
    string? Email,
    string Role
);
