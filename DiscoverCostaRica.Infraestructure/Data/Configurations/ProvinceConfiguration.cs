using DiscoverCostaRica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Infraestructure.Data.Configurations;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("Province");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Name).IsRequired();

        builder.HasMany(p => p.Cantons)
        .WithOne(p => p.Province)
        .HasForeignKey(p => p.ProvinceId);

        builder.HasMany(p => p.Attractions)
        .WithOne(a => a.Province)
        .HasForeignKey(a => a.ProviceId);
    }
}