using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Web.Domain.Entities;

namespace ThreeCats.Web.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasColumnType("text");

        builder.Property(s => s.Price)
            .HasPrecision(10, 2)
            .IsRequired();
    }
}
