using ThreeCats.Web.Domain.Enums;

namespace ThreeCats.Web.Domain.Entities;

public class Pet
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public PetSex Sex { get; set; }
    public DateOnly BirthDate { get; set; }
    public decimal? Weight { get; set; }
    public string? Notes { get; set; }
}
