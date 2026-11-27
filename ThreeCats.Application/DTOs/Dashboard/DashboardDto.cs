namespace ThreeCats.Application.DTOs.Dashboard;

public sealed record DashboardDto(
    int TodayAppointmentsCount,
    int ClientsCount,
    int PetsCount,
    int DoctorsCount,
    IReadOnlyList<DashboardAppointmentDto> UpcomingAppointments
);
