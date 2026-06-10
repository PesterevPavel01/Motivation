using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Motivation.Infrastructure.Configurations.Base;

namespace Motivation.Infrastructure.Configurations;

public sealed class ExtraPartConfiguration : EntityConfiguration<ExtraPart>
{
    protected override void AddBuilder(EntityTypeBuilder<ExtraPart> builder)
    {
        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder.Property(x => x.ValidFrom).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder.Property(x => x.ValidTo).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder
            .Property(x => x.ExtraPartValue)
            .IsRequired()
            .HasConversion(x => x.Value, x => ExtraPartValue.Create(x).Result);

    }

    protected override string TableName()
        => "extra_parts";
}
