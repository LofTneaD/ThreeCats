namespace ThreeCats.Application.DTOs.Doctors;

public sealed record UpdateDoctorDto(
    Guid Id,
    Guid UserId,
    string Specialization,
    string Description,
    bool IsActive);
