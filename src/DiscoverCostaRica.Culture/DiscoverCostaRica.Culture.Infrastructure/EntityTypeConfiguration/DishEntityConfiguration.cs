using DiscoverCostaRica.Culture.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Culture.Infraestructure.EntityTypeConfiguration;

internal class DishEntityConfiguration : IEntityTypeConfiguration<DishModel>
{
    public void Configure(EntityTypeBuilder<DishModel> builder)
    {
        builder.ToTable("Culture.Dish");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.HasIndex(p => p.Id).IsUnique();
        builder.Property(p => p.Name).IsRequired();
    }
}
