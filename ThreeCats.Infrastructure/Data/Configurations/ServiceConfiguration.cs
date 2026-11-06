using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Domain.Entities;

namespace ThreeCats.Infrastructure.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_Service_Price_NonNegative", "\"Price\" >= 0"));

        builder.Property(s => s.Price)
            .HasPrecision(10, 2)
            .IsRequired();
    }
}
