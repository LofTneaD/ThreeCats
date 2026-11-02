using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Domain.Entities;

namespace ThreeCats.Infrastructure.Data.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.HasKey(v => v.Id);

        builder.HasIndex(v => v.AppointmentId)
            .IsUnique();

        builder.HasOne<Appointment>()
            .WithOne()
            .HasForeignKey<Visit>(v => v.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Doctor>()
            .WithMany()
            .HasForeignKey(v => v.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
