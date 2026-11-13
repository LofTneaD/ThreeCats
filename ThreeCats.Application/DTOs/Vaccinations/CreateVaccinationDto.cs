namespace ThreeCats.Application.DTOs.Vaccinations;

public sealed record CreateVaccinationDto(
    Guid PetId,
    Guid? VisitId,
    string VaccineName,
    string BatchNumber,
    DateOnly VaccinatedAt,
    DateOnly? NextDueAt,
    string Notes);
