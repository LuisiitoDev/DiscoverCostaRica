using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Volcano.Infrastructure.Persistence;

/// <summary>Persistence shape of the Volcano table; the domain entity stays persistence-ignorant.</summary>
internal sealed class VolcanoRow
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public int ProvinceId { get; set; }

    public int CantonId { get; set; }

    public int? DistrictId { get; set; }
}

/// <summary>Same relational model as the existing migrations (guarded by MigrationTests).</summary>
internal sealed class VolcanoRowConfiguration : IEntityTypeConfiguration<VolcanoRow>
{
    public void Configure(EntityTypeBuilder<VolcanoRow> builder)
    {
        builder.ToTable("Volcano.Volcano");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();
        builder.HasIndex(v => v.Id).IsUnique();
        builder.Property(v => v.Name).IsRequired();
        builder.Property(v => v.Description).IsRequired();
        builder.Property(v => v.ProvinceId).IsRequired();
        builder.Property(v => v.CantonId).IsRequired();
        builder.Property(v => v.DistrictId).IsRequired(false);
    }
}
