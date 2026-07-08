using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Infrastructure.Configurations.Base;

namespace Motivation.AnalyticsCore.Infrastructure.Configurations;

public class CustomerTransactionConfiguration : EntityConfiguration<CustomerTransaction>
{
    protected override void AddBuilder(EntityTypeBuilder<CustomerTransaction> builder)
    {
        builder
            .Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                x => x.ToRussian(),
                x=> CustomerTransactionTypeMapper.FromRussian(x)
            );

        builder.Property(x => x.AsOfDate).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder
            .Property(x => x.TotalAmount)
            .IsRequired(false)
            .HasConversion(
                x => x != null ? x.Value : (decimal?)null,
                x => x.HasValue ? AmountValue.Create(x.Value).Result : null);

        builder
            .Property(x => x.Responsible)
            .IsRequired()
            .HasMaxLength(ResponsibleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => ResponsibleValue.Create(x).Result);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.CustomerTransactions);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.CustomerTransactions);

        builder.HasMany(x => x.CustomerTransactionProducts)
            .WithOne(x => x.CustomerTransaction);

        builder.HasIndex(x => x.Type);

        builder.HasIndex(x => x.Responsible);
    }

    protected override string TableName()
        => "customer_transaction";
}
