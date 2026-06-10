using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Motivation.Infrastructure.Configurations.Base;

namespace Motivation.Infrastructure.Configurations;

internal sealed class PositionConfiguration : EntityConfiguration<Position>
{
    protected override void AddBuilder(EntityTypeBuilder<Position> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder.HasIndex(x => x.Title);

        builder
            .Property(x => x.BaseSalary)
            .IsRequired()
            .HasConversion(x => x.Value, x => SalaryValue.Create(x).Result);

        builder
            .Property(x => x.WorkWeekType)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion<string>();

        builder
            .HasMany(x => x.EmployeePositions)
            .WithOne(x => x.Position)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasMany(x => x.PositionKpiHistory)
            .WithOne(x => x.Position)
            .HasPrincipalKey(x => x.Id);

        builder.OwnsOne(x => x.MotivationPart, motivationBuilder =>
        {
            motivationBuilder.ToJson();

            motivationBuilder
                .Property(x => x.MotivationPartValue)
                .IsRequired()
                .HasConversion(x => x.Value, x => MotivationPartValue.Create(x).Result);
        });
    }

    protected override string TableName()
     => "positions";
}
