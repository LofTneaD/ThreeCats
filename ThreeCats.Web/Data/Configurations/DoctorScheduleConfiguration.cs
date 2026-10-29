using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ThreeCats.Web.Domain.Entities;

namespace ThreeCats.Web.Data.Configurations;

public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.DayOfWeek)
            .HasConversion(new ValueConverter<DayOfWeek, int>(day => ((int)day + 6) % 7,
            value => (DayOfWeek)((value + 1) % 7))).IsRequired();

        builder.Property(s => s.StartTime).HasColumnType("time without time zone");

        builder.Property(s => s.EndTime).HasColumnType("time without time zone");

        builder.HasOne<Doctor>()
            .WithMany()
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
