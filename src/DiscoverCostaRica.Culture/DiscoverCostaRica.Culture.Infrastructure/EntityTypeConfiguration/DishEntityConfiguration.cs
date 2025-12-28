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
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.HasIndex(p => p.Id).IsUnique();
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Ingredients).IsRequired();
        builder.Property(p => p.Preparation).IsRequired();
        builder.Property(p => p.ImageUrl).IsRequired(false);
    }
}
