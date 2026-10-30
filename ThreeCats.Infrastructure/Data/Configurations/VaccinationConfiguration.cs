using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Domain.Entities;

namespace ThreeCats.Infrastructure.Data.Configurations;

public class VaccinationConfiguration : IEntityTypeConfiguration<Vaccination>
{
    public void Configure(EntityTypeBuilder<Vaccination> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.VaccineName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(v => v.BatchNumber)
            .HasMaxLength(100);

        builder.Property(v => v.VaccinatedAt)
            .HasColumnType("date");

        builder.Property(v => v.NextDueAt)
            .HasColumnType("date");

        builder.Property(v => v.Notes)
            .HasColumnType("text");

        builder.HasOne<Pet>()
            .WithMany()
            .HasForeignKey(v => v.PetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Visit>()
            .WithMany()
            .HasForeignKey(v => v.VisitId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
