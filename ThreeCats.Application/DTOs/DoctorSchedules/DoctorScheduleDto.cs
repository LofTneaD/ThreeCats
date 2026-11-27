namespace ThreeCats.Application.DTOs.DoctorSchedules;

public sealed record DoctorScheduleDto(
    Guid Id,
    Guid DoctorId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive);
