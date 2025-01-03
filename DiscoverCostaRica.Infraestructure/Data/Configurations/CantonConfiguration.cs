using DiscoverCostaRica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Infraestructure.Data.Configurations;

public class CantonConfiguration : IEntityTypeConfiguration<Canton>
{
    public void Configure(EntityTypeBuilder<Canton> builder)
    {
        builder.ToTable("Canton");
        builder.HasKey(c => c.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired();
        builder.HasMany(c => c.Districts)
        .WithOne(c => c.Canton)
        .HasForeignKey(c => c.CantonId);
    }
}