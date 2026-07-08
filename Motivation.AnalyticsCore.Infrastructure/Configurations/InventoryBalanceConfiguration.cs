using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Infrastructure.Configurations;

public class InventoryBalanceConfiguration : IEntityTypeConfiguration<InventoryBalance>
{
    public void Configure(EntityTypeBuilder<InventoryBalance> builder)
    {
        builder.ToTable("inventory_balances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAt).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder.Property(x => x.UpdatedAt).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder
            .Property(x => x.TotalQuantity)
            .IsRequired()
            .HasConversion(x => x.Value, x => QuantityValue.Create(x).Result);

        builder
            .Property(x => x.TotalAmount)
            .IsRequired()
            .HasConversion(
                x => x != null ? x.Value : (decimal?)null,
                x => x.HasValue ? AmountValue.Create(x.Value).Result : null);

        builder.Property(x => x.AsOfDate).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.InventoryBalances);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.InventoryBalances);
    }
}
