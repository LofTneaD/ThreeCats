namespace ThreeCats.Application.DTOs.Doctors;

public sealed record CreateDoctorDto(
    string LastName,
    string FirstName,
    string? MiddleName,
    string PhoneNumber,
    string? Email,
    string Password,
    string Specialization,
    string Description,
    bool IsActive
);
