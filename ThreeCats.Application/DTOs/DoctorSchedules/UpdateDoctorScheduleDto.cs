namespace ThreeCats.Application.DTOs.DoctorSchedules;

public sealed record UpdateDoctorScheduleDto(
    Guid Id,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive);
