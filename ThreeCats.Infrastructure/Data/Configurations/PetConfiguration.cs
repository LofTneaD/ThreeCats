using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThreeCats.Domain.Entities;

namespace ThreeCats.Infrastructure.Data.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();

        builder.Property(p => p.Species).HasMaxLength(50).IsRequired();

        builder.Property(p => p.Breed).HasMaxLength(100).IsRequired();

        builder.Property(p => p.BirthDate).HasColumnType("date");

        builder.Property(p => p.Weight).HasPrecision(5, 2).HasComment("Вес питомца в килограммах");

        builder.ToTable(t => t.HasCheckConstraint("CK_Pet_Weight_NonNegative", "\"Weight\" >= 0"));

        builder.HasOne<Client>(p => p.Client).WithMany(c => c.Pets).HasForeignKey(p=>p.ClientId).OnDelete(DeleteBehavior.Cascade);
    }
}
