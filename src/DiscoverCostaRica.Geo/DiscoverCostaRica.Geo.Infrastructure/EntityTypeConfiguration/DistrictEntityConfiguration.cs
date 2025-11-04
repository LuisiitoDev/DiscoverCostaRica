using DiscoverCostaRica.Geo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Geo.Infraestructure.EntityTypeConfiguration;

public class DistrictEntityConfiguration : IEntityTypeConfiguration<DistrictModel>
{
    public void Configure(EntityTypeBuilder<DistrictModel> builder)
    {
        builder.ToTable("Geo.District");

        builder.HasKey(d => d.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(d => d.Name).IsRequired();
    }
}
