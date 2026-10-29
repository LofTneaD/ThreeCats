namespace ThreeCats.Web.Domain.Entities;

public class VisitResult
{
    public Guid Id { get; set; }
    public Guid VisitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string ReferenceRange { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string FileUrl { get; set; } = string.Empty;
}
