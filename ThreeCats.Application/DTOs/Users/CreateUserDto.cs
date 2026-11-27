namespace ThreeCats.Application.DTOs.Users;

public sealed record CreateUserDto(
    string LastName,
    string FirstName,
    string? MiddleName,
    string PhoneNumber,
    string? Email,
    string Password,
    string Role
);
