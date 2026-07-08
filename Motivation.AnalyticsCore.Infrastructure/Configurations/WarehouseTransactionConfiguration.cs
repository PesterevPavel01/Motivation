using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Infrastructure.Configurations.Base;

namespace Motivation.AnalyticsCore.Infrastructure.Configurations;

public class WarehouseTransactionConfiguration : EntityConfiguration<WarehouseTransaction>
{
    protected override void AddBuilder(EntityTypeBuilder<WarehouseTransaction> builder)
    {
        builder
            .Property(x => x.Quantity)
            .IsRequired(false)
            .HasConversion(
                x => x != null ? x.Value : (decimal?)null,
                x => x.HasValue ? QuantityValue.Create(x.Value).Result : null);

        builder
            .Property(x => x.Amount)
            .IsRequired(false)
            .HasConversion(
                x => x != null ? x.Value : (decimal?)null,
                x => x.HasValue ? AmountValue.Create(x.Value).Result : null);

        builder
            .Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                x => x.ToRussian(),
                x => WarehouseTransactionTypeMapper.FromRussian(x)
            );

        builder.Property(x => x.AsOfDate).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.WarehouseTransactions);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.WarehouseTransactions);

        builder.HasIndex(x => x.Type);
    }

    protected override string TableName()
        => "warehouse_transaction";
}
