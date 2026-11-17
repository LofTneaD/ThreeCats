namespace ThreeCats.Application.DTOs.Doctors;

public sealed record UpdateDoctorDto(
    Guid Id,
    string LastName,
    string FirstName,
    string? MiddleName,
    string PhoneNumber,
    string? Email,
    string Specialization,
    string Description,
    bool IsActive
);
