using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Domain.Entities;

namespace ThreeCats.Infrastructure.Data.Configurations;

public class VisitResultConfiguration : IEntityTypeConfiguration<VisitResult>
{
    public void Configure(EntityTypeBuilder<VisitResult> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(r => r.Result)
            .HasColumnType("text");

        builder.Property(r => r.ReferenceRange)
            .HasMaxLength(150);

        builder.Property(r => r.PerformedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(r => r.FileUrl)
            .HasColumnType("text");

        builder.HasOne<Visit>()
            .WithMany()
            .HasForeignKey(r => r.VisitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
