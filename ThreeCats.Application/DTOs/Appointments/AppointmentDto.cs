using ThreeCats.Domain.Enums;

namespace ThreeCats.Application.DTOs.Appointments;

public sealed record AppointmentDto(
    Guid Id,
    Guid ClientId,
    Guid PetId,
    Guid DoctorId,
    Guid ServiceId,
    Guid CreatedByUserId,
    DateTime StartAt,
    DateTime EndAt,
    DateTime CreatedAt,
    AppointmentStatus Status,
    string Comment);
