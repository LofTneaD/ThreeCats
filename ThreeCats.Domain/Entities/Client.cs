namespace ThreeCats.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
