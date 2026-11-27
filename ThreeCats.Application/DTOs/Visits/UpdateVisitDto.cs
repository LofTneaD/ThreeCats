namespace ThreeCats.Application.DTOs.Visits;

public sealed record UpdateVisitDto(
    Guid Id,
    Guid DoctorId,
    string Complaints,
    string Diagnosis,
    string Treatment,
    string Recommendations,
    string Notes);
