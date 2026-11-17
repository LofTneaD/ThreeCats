namespace ThreeCats.Application.DTOs.Doctors;

public sealed record DoctorDto(
    Guid Id,
    Guid UserId,
    string LastName,
    string FirstName,
    string? MiddleName,
    string? PhoneNumber,
    string? Email,
    string Specialization,
    string Description,
    bool IsActive
);
