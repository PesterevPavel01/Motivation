using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Infrastructure.Configurations.Base;

namespace Motivation.AnalyticsCore.Infrastructure.Configyrations;

public class CustomerCunfiguration : EntityConfiguration<Customer>
{
    protected override void AddBuilder(EntityTypeBuilder<Customer> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder
            .Property(x => x.FullName)
            .IsRequired(false)
            .HasMaxLength(FullNameValue.MaxFullNameLength)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? FullNameValue.Create(x).Result : null);
       
        builder
            .Property(x => x.Manager)
            .IsRequired(false)
            .HasMaxLength(ManagerValue.MaxManagerLength)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? ManagerValue.Create(x).Result : null);

        builder.HasOne(x => x.MasterCustomer)
            .WithMany(x => x.SlaveCustomers)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CustomerGroup)
            .WithMany(x => x.Customers)
            .HasPrincipalKey(x => x.Id);

        builder.HasMany(x => x.Receivables)
            .WithOne(x => x.Customer)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.CustomerTransactions)
            .WithOne(x => x.Customer)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.FullName);
    }

    protected override string TableName()
        => "customers";
}
