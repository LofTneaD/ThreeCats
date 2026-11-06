using System.ComponentModel;
using ThreeCats.Domain.Enums;

namespace ThreeCats.Domain.Entities;

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
    public Client Client { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
}
