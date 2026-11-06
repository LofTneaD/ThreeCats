using ThreeCats.Domain.Enums;

namespace ThreeCats.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid PetId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public AppointmentStatus Status { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Doctor Doctor { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Pet Pet { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Visit? Visit { get; set; }
}
