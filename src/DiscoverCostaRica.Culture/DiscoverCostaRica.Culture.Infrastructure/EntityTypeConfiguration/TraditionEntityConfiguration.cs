using DiscoverCostaRica.Culture.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Culture.Infraestructure.EntityTypeConfiguration;

public class TraditionEntityConfiguration : IEntityTypeConfiguration<TraditionModel>
{
    public void Configure(EntityTypeBuilder<TraditionModel> builder)
    {
        builder.ToTable("Culture.Tradition");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.HasIndex(p => p.Id).IsUnique();
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.ImageUrl).IsRequired(false);
    }
}
