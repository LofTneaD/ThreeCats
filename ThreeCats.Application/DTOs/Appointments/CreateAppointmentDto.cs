using ThreeCats.Domain.Enums;

namespace ThreeCats.Application.DTOs.Appointments;

public sealed record CreateAppointmentDto(
    Guid ClientId,
    Guid PetId,
    Guid DoctorId,
    Guid ServiceId,
    Guid CreatedByUserId,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status,
    string Comment);
