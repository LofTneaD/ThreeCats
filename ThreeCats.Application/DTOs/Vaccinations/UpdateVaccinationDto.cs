namespace ThreeCats.Application.DTOs.Vaccinations;

public sealed record UpdateVaccinationDto(
    Guid Id,
    Guid? VisitId,
    string VaccineName,
    string BatchNumber,
    DateOnly VaccinatedAt,
    DateOnly? NextDueAt,
    string Notes);
