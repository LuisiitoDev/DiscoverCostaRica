using DiscoverCostaRica.Beaches.Application.Ports;
using DiscoverCostaRica.Beaches.Domain;
using DiscoverCostaRica.Beaches.Infrastructure.Context;
using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Beaches.Infrastructure.Persistence;

internal sealed class BeachRow
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }
}

/// <summary>Same relational model as the existing migrations (guarded by MigrationTests).</summary>
internal sealed class BeachRowConfiguration : IEntityTypeConfiguration<BeachRow>
{
    public void Configure(EntityTypeBuilder<BeachRow> builder)
    {
        builder.ToTable("Beach.Beach");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.Name).IsRequired().HasMaxLength(Beach.MaxNameLength);
        builder.Property(b => b.Description).IsRequired().HasMaxLength(Beach.MaxDescriptionLength);
    }
}

[TransientService]
internal sealed class BeachReadRepository(BeachContext context) : IBeachReadRepository
{
    public async Task<IReadOnlyList<Beach>> GetAllAsync(CancellationToken cancellationToken) =>
        [.. (await context.Set<BeachRow>().AsNoTracking().OrderBy(b => b.Id).ToListAsync(cancellationToken))
            .Select(row => Beach.Create(row.Id, row.Name, row.Description))];
}
