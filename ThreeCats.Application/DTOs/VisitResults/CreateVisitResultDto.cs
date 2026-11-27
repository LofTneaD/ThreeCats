namespace ThreeCats.Application.DTOs.VisitResults;

public sealed record CreateVisitResultDto(
    Guid VisitId,
    string Name,
    string Result,
    string? ReferenceRange,
    DateTime PerformedAt,
    string? FileUrl);
