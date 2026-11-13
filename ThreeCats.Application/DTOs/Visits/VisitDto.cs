namespace ThreeCats.Application.DTOs.Visits;

public sealed record VisitDto(
    Guid Id,
    Guid AppointmentId,
    Guid DoctorId,
    string Complaints,
    string Diagnosis,
    string Treatment,
    string Recommendations,
    string Notes,
    DateTime CreatedAt);
