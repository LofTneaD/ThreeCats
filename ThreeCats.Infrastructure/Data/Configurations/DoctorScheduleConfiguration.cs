using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ThreeCats.Domain.Entities;

namespace ThreeCats.Infrastructure.Data.Configurations;

public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.DayOfWeek)
            .HasConversion(new ValueConverter<DayOfWeek, int>(day => ((int)day + 6) % 7,
            value => (DayOfWeek)((value + 1) % 7))).IsRequired();

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_DoctorSchedule_EndTime_After_StartTime",
            "\"EndTime\" > \"StartTime\""));

        builder.HasOne<Doctor>(s=>s.Doctor)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
