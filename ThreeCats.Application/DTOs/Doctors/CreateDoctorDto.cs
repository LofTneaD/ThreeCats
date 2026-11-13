namespace ThreeCats.Application.DTOs.Doctors;

public sealed record CreateDoctorDto(
    Guid UserId,
    string Specialization,
    string Description,
    bool IsActive);
