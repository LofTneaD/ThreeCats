using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Web.Domain.Entities;

namespace ThreeCats.Web.Data.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.HasKey(v => v.Id);

        builder.HasIndex(v => v.AppointmentId)
            .IsUnique();

        builder.Property(v => v.Complaints)
            .HasColumnType("text");

        builder.Property(v => v.Diagnosis)
            .HasColumnType("text");

        builder.Property(v => v.Treatment)
            .HasColumnType("text");

        builder.Property(v => v.Recommendations)
            .HasColumnType("text");

        builder.Property(v => v.Notes)
            .HasColumnType("text");

        builder.Property(v => v.CreatedAt)
            .HasColumnType("timestamp with time zone");

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
