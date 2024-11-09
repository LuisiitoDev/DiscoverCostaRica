
using DiscoverCostaRica.Domain.Entities.Direction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverCostaRica.Infraestructure.Data.Configurations;

public class ProvinceDetailConfiguration : IEntityTypeConfiguration<ProvinceDetail>
{
    public void Configure(EntityTypeBuilder<ProvinceDetail> builder)
    {
        builder.ToTable("ProvinceDetail");
        builder.HasKey(pd => pd.Id);
        builder.Property(pd => pd.History).IsRequired();
        builder.Property(pd => pd.Map).IsRequired();
        builder.HasOne(pd => pd.Province).WithOne(p => p.ProvinceDetail);
    }
}