namespace ThreeCats.Domain.Entities;

public class Vaccination
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Guid? VisitId { get; set; }
    public string VaccineName { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly VaccinatedAt { get; set; }
    public DateOnly? NextDueAt { get; set; }
    public string Notes { get; set; } = string.Empty;
}
