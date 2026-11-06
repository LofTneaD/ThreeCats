using ThreeCats.Domain.Enums;

namespace ThreeCats.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
}
