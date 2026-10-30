using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Identity;

namespace ThreeCats.Infrastructure.Data.Configurations;

public class ClientConfigurations : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.UserId).IsUnique();

        builder.Property(c => c.Address).HasColumnType("text");

        builder.Property(c => c.Notes).HasColumnType("text");

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Client>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
