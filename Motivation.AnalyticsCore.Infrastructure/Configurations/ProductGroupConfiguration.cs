using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Infrastructure.Configurations.Base;

namespace Motivation.AnalyticsCore.Infrastructure.Configurations;

public class ProductGroupConfiguration : EntityConfiguration<ProductGroup>
{
    protected override void AddBuilder(EntityTypeBuilder<ProductGroup> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder.HasOne(x => x.ParentGroup)
            .WithMany(x => x.ChildGroups)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Products)
            .WithOne(x => x.ProductGroup)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.Title);
    }

    protected override string TableName()
        => "product_group";
}
