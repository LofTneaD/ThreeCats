namespace ThreeCats.Application.DTOs.Visits;

public sealed record CreateVisitDto(
    Guid AppointmentId,
    Guid DoctorId,
    string Complaints,
    string Diagnosis,
    string Treatment,
    string Recommendations,
    string Notes);
