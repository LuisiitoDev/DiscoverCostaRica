using DiscoverCostaRica.Geo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Geo.Infraestructure.EntityTypeConfiguration;

public class ProvinceEntityConfiguration : IEntityTypeConfiguration<ProvinceModel>
{
    public void Configure(EntityTypeBuilder<ProvinceModel> builder)
    {
        builder.ToTable("Geo.Province");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.HasIndex(p => p.Id).IsUnique();

        builder.Property(p => p.Name).IsRequired();

        builder.HasMany(p => p.Cantons).WithOne(p => p.Province)
        .HasForeignKey(p => p.ProvinceId);
    }
}
