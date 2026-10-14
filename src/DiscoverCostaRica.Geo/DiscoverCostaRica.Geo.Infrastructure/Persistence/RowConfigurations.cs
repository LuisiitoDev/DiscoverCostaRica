using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Geo.Infrastructure.Persistence;

// Same relational model as the existing migrations: tables, keys, indexes and cascade FKs.
// MigrationTests (HasPendingModelChanges) guards that nothing drifts.

internal sealed class ProvinceRowConfiguration : IEntityTypeConfiguration<ProvinceRow>
{
    public void Configure(EntityTypeBuilder<ProvinceRow> builder)
    {
        builder.ToTable("Geo.Province");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.HasIndex(p => p.Id).IsUnique();
        builder.Property(p => p.Name).IsRequired();
    }
}

internal sealed class CantonRowConfiguration : IEntityTypeConfiguration<CantonRow>
{
    public void Configure(EntityTypeBuilder<CantonRow> builder)
    {
        builder.ToTable("Geo.Canton");
        builder.HasKey(c => new { c.Id, c.ProvinceId });
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired();
        builder.HasOne<ProvinceRow>()
            .WithMany()
            .HasForeignKey(c => c.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}

internal sealed class DistrictRowConfiguration : IEntityTypeConfiguration<DistrictRow>
{
    public void Configure(EntityTypeBuilder<DistrictRow> builder)
    {
        builder.ToTable("Geo.District");
        builder.HasKey(d => new { d.Id, d.CantonId, d.CantonProvinceId });
        builder.Property(d => d.Id).ValueGeneratedNever();
        builder.Property(d => d.Name).IsRequired();
        builder.HasOne<CantonRow>()
            .WithMany()
            .HasForeignKey(d => new { d.CantonId, d.CantonProvinceId })
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
