namespace ThreeCats.Domain.Entities;

public class Visit
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid DoctorId { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public Pet Pet { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public ICollection<VisitResult> VisitResults { get; set; } = new List<VisitResult>();
}
