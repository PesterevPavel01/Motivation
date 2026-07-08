using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Infrastructure.Configurations.Base;

namespace Motivation.AnalyticsCore.Infrastructure.Configyrations;

internal class CompanyConfiguration : EntityConfiguration<Company>
{
    protected override void AddBuilder(EntityTypeBuilder<Company> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder.HasMany(x => x.Receivables)
            .WithOne(x => x.Company);

        builder.HasMany(x => x.CustomerTransactions)
            .WithOne(x => x.Company);

        builder.HasIndex(x => x.Title);
    }

    protected override string TableName()
        => "companies";
}
