using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Web.Domain.Entities;

namespace ThreeCats.Web.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);

        builder.HasIndex(d => d.UserId);

        builder.Property(d => d.Specialization).HasMaxLength(150).IsRequired();

        builder.Property(d=>d.Description).HasColumnType("text");

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Doctor>(d=>d.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
