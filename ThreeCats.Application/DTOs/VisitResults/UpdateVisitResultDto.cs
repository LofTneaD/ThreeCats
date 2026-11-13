namespace ThreeCats.Application.DTOs.VisitResults;

public sealed record UpdateVisitResultDto(
    Guid Id,
    string Name,
    string Result,
    string? ReferenceRange,
    DateTime PerformedAt,
    string? FileUrl);
