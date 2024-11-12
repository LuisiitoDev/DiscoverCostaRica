using DiscoverCostaRica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Infraestructure.Data.Configurations;
public class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable("Dish");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).UseIdentityColumn();
        builder.Property(d => d.Name).IsRequired();
        builder.Property(d => d.Description).IsRequired();
    }
}