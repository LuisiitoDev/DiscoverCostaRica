using DiscoverCostaRica.Geo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Geo.Infraestructure.EntityTypeConfiguration;

public class CantonEntityConfiguration : IEntityTypeConfiguration<CantonModel>
{
    public void Configure(EntityTypeBuilder<CantonModel> builder)
    {
        builder.ToTable("Geo.Canton");

        builder.HasKey(c => new { c.Id, c.ProvinceId });
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired();


        builder
            .HasMany(c => c.Districts)
            .WithOne(c => c.Canton)
            .HasForeignKey(c => new { c.CantonId, c.CantonProvinceId });
    }
}
