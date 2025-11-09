using DiscoverCostaRica.Geo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Geo.Infraestructure.EntityTypeConfiguration;

public class DistrictEntityConfiguration : IEntityTypeConfiguration<DistrictModel>
{
    public void Configure(EntityTypeBuilder<DistrictModel> builder)
    {
        builder.ToTable("Geo.District");

        builder.HasKey(d => new { d.Id, d.CantonId, d.CantonProvinceId });
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(d => d.Name).IsRequired();

        builder.HasOne(d => d.Canton)
               .WithMany(c => c.Districts)
               .HasForeignKey(d => new { d.CantonId, d.CantonProvinceId });
    }
}
