using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Motivation.Infrastructure.Configurations.Base;

namespace Motivation.Infrastructure.Configurations;

internal class MeasurementUnitConfiguration : EntityConfiguration<MeasurementUnit>
{
    protected override void AddBuilder(EntityTypeBuilder<MeasurementUnit> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder.HasIndex(x => x.Title);
    }

    protected override string TableName()
     => "measurement_unit";
}