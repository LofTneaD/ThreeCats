using ThreeCats.Domain.Enums;

namespace ThreeCats.Application.DTOs.Appointments;

public sealed record UpdateAppointmentDto(
    Guid Id,
    Guid ClientId,
    Guid PetId,
    Guid DoctorId,
    Guid ServiceId,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status,
    string Comment);
