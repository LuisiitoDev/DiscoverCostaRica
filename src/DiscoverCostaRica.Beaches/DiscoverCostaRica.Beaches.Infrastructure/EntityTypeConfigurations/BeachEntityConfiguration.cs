using DiscoverCostaRica.Beaches.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Beaches.Infrastructure.EntityTypeConfigurations;

public class BeachEntityConfiguration : IEntityTypeConfiguration<BeachModel>
{
    public void Configure(EntityTypeBuilder<BeachModel> builder)
    {
        builder.ToTable("Beach.Beach");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.Name).IsRequired().HasMaxLength(1000);
        builder.Property(b => b.Description).IsRequired().HasMaxLength(10000);
    }
}
