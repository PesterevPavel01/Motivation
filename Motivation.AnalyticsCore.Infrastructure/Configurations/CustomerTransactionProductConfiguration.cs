using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Infrastructure.Configurations;

public class CustomerTransactionProductConfiguration : IEntityTypeConfiguration<CustomerTransactionProduct>
{
    public void Configure(EntityTypeBuilder<CustomerTransactionProduct> builder)
    {
        builder.ToTable("customer_transaction_products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasConversion(x => x.Value, x => QuantityValue.Create(x).Result);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasConversion(x => x.Value, x => AmountValue.Create(x).Result);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.CustomerTransactionProducts)
            .HasPrincipalKey(x => x.Id);
    }
}