namespace ThreeCats.Application.DTOs.VisitResults;

public sealed record VisitResultDto(
    Guid Id,
    Guid VisitId,
    string Name,
    string Result,
    string? ReferenceRange,
    DateTime PerformedAt,
    string? FileUrl);
