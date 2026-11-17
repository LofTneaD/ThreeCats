using ThreeCats.Domain.Enums;

namespace ThreeCats.Application.DTOs.Dashboard;

public sealed record DashboardAppointmentDto(
    Guid Id,
    DateTime StartAt,
    string PetName,
    string PetSpecies,
    string ClientFullName,
    string DoctorFullName,
    AppointmentStatus Status
);
