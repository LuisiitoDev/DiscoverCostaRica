using DiscoverCostaRica.BuildingBlocks.Application.DependencyInjection;
using DiscoverCostaRica.Culture.Application.Ports;
using DiscoverCostaRica.Culture.Domain;
using DiscoverCostaRica.Culture.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Culture.Infrastructure.Persistence;

internal sealed class TraditionRow
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public string? ImageUrl { get; set; }
}

internal sealed class DishRow
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Ingredients { get; set; }

    public required string Preparation { get; set; }

    public string? ImageUrl { get; set; }
}

// Same relational model as the existing migrations (guarded by MigrationTests).

internal sealed class TraditionRowConfiguration : IEntityTypeConfiguration<TraditionRow>
{
    public void Configure(EntityTypeBuilder<TraditionRow> builder)
    {
        builder.ToTable("Culture.Tradition");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.HasIndex(t => t.Id).IsUnique();
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.ImageUrl).IsRequired(false);
    }
}

internal sealed class DishRowConfiguration : IEntityTypeConfiguration<DishRow>
{
    public void Configure(EntityTypeBuilder<DishRow> builder)
    {
        builder.ToTable("Culture.Dish");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();
        builder.HasIndex(d => d.Id).IsUnique();
        builder.Property(d => d.Name).IsRequired();
        builder.Property(d => d.Description).IsRequired();
        builder.Property(d => d.Ingredients).IsRequired();
        builder.Property(d => d.Preparation).IsRequired();
        builder.Property(d => d.ImageUrl).IsRequired(false);
    }
}

[TransientService]
internal sealed class CultureReadRepository(CultureContext context) : ICultureReadRepository
{
    public async Task<IReadOnlyList<Tradition>> GetTraditionsAsync(CancellationToken cancellationToken) =>
        [.. (await context.Set<TraditionRow>().AsNoTracking().OrderBy(t => t.Id).ToListAsync(cancellationToken))
            .Select(row => Tradition.Create(row.Id, row.Name, row.Description, row.ImageUrl))];

    public async Task<IReadOnlyList<Dish>> GetDishesAsync(CancellationToken cancellationToken) =>
        [.. (await context.Set<DishRow>().AsNoTracking().OrderBy(d => d.Id).ToListAsync(cancellationToken))
            .Select(row => Dish.Create(row.Id, row.Name, row.Description, row.Ingredients, row.Preparation, row.ImageUrl))];
}
