namespace ThreeCats.Application.DTOs.Doctors;

public sealed record DoctorDto(
    Guid Id,
    Guid UserId,
    string Specialization,
    string Description,
    bool IsActive
);
