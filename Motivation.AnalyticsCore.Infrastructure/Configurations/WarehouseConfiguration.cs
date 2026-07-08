using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Infrastructure.Configurations.Base;

namespace Motivation.AnalyticsCore.Infrastructure.Configurations;

public class WarehouseConfiguration : EntityConfiguration<Warehouse>
{
    protected override void AddBuilder(EntityTypeBuilder<Warehouse> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder.HasMany(x => x.InventoryBalances)
            .WithOne(x => x.Warehouse);

        builder.HasMany(x => x.WarehouseTransactions)
            .WithOne(x => x.Warehouse);
    }

    protected override string TableName()
        => "warehouses";
}
