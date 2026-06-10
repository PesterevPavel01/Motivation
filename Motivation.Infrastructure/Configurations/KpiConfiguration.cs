using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Motivation.Infrastructure.Configurations.Base;

namespace Motivation.Infrastructure.Configurations;

internal class KpiConfiguration : EntityConfiguration<Kpi>
{
    protected override void AddBuilder(EntityTypeBuilder<Kpi> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder.HasIndex(x => x.Title);

        builder
            .Property(x => x.Abbreviation)
            .IsRequired()
            .HasMaxLength(AbbreviationValue.MaxLength)
            .HasConversion(x => x.Value, x => AbbreviationValue.Create(x).Result);

        builder
            .Property(x => x.CalculationType)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();

        builder
            .HasOne(x => x.MeasurementUnit)
            .WithMany(x => x.Kpis)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasMany(x => x.Filters);
    }

    protected override string TableName()
     => "kpis";
}
