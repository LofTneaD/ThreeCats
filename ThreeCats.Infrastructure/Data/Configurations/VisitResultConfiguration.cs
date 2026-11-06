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

        builder.Property(r => r.ReferenceRange)
            .HasMaxLength(150);

        builder.HasOne<Visit>(r => r.Visit)
            .WithMany(v => v.VisitResults)
            .HasForeignKey(r => r.VisitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
