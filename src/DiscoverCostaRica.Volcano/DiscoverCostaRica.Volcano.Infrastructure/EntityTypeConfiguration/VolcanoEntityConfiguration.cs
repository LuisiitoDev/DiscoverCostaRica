using DiscoverCostaRica.VolcanoService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.VolcanoService.Infraestructure.EntityTypeConfiguration;

public class VolcanoEntityConfiguration : IEntityTypeConfiguration<VolcanoModel>
{
    public void Configure(EntityTypeBuilder<VolcanoModel> builder)
    {
        builder.ToTable("Volcano.Volcano");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.HasIndex(p => p.Id).IsUnique();

        builder.Property(p => p.Name).IsRequired();

        builder.Property(p => p.ProvinceId).IsRequired();
        builder.Property(p => p.CantonId).IsRequired();
        builder.Property(p => p.DistrictId).IsRequired(false);
    }
}
