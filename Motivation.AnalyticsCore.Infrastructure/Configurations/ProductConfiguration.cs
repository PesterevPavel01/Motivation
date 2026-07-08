using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Infrastructure.Configurations.Base;

namespace Motivation.AnalyticsCore.Infrastructure.Configurations;

public class ProductConfiguration : EntityConfiguration<Product>
{
    protected override void AddBuilder(EntityTypeBuilder<Product> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder
            .Property(x => x.Purchaser)
            .IsRequired(false)
            .HasMaxLength(PurchaserValue.MaxTitleLength)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? PurchaserValue.Create(x).Result : null);

        builder
            .Property(x => x.Status)
            .IsRequired(false)
            .HasMaxLength(StatusValue.MaxTitleLength)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? StatusValue.Create(x).Result : null);

        builder.HasOne(x => x.ProductGroup)
            .WithMany(x => x.Products)
            .HasPrincipalKey(x => x.Id);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.Products)
            .HasPrincipalKey(x => x.Id);

        builder.HasMany(x => x.WarehouseTransactions)
            .WithOne(x => x.Product)
            .HasPrincipalKey(x => x.Id);
    }

    protected override string TableName()
        => "products";
}
