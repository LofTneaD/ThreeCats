namespace ThreeCats.Application.DTOs.Users;

public sealed record UserDto(
    Guid Id,
    string? UserName,
    string? Email,
    string? PhoneNumber,
    string Role,
    string LastName,
    string FirstName,
    string? MiddleName
);
