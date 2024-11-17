using DiscoverCostaRica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Infraestructure.Data.Configurations;
public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Description).IsRequired();
        builder.Property(c => c.CountryCode).IsRequired();
        builder.HasOne(c => c.Capital)
            .WithOne(p => p.Country)
            .HasForeignKey<Province>(c => c.CountryId);
    }
}
