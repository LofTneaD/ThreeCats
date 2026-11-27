namespace ThreeCats.Application.DTOs.DoctorSchedules;

public sealed record CreateDoctorScheduleDto(
    Guid DoctorId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive);
