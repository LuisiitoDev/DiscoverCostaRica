using DiscoverCostaRica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DiscoverCostaRica.Infraestructure.Data.Configurations;

public class BeachConfiguration : IEntityTypeConfiguration<Beach>
{
    public void Configure(EntityTypeBuilder<Beach> builder)
    {
        builder.ToTable("Beach");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.Name).IsRequired().HasMaxLength(1000);
        builder.Property(b => b.Description).IsRequired().HasMaxLength(10000);
    }
}