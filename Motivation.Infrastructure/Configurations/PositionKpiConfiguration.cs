using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.Infrastructure.Configurations;

internal sealed class PositionKpiConfiguration : IEntityTypeConfiguration<PositionKpi>
{
    public void Configure(EntityTypeBuilder<PositionKpi> builder)
    {
        builder.ToTable("position_kpis");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAt).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder.Property(x => x.UpdatedAt).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

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
            .Property(x => x.Target)
            .IsRequired()
            .HasConversion(x => x.Value, x => TargetValue.Create(x).Result);

        builder
            .Property(x => x.Fact)
            .IsRequired()
            .HasConversion(x => x.Value, x => FactValue.Create(x).Result);

        builder
            .Property(x => x.Achievement)
            .IsRequired()
            .HasConversion(x => x.Value, x => AchievementValue.Create(x).Result);

        builder
            .Property(x => x.BonusAmount)
            .IsRequired()
            .HasConversion(x => x.Value, x => BonusAmountValue.Create(x).Result);

        builder
            .Property(x => x.Weight)
            .IsRequired()
            .HasConversion(x => x.Value, x => WeightValue.Create(x).Result);

        builder
            .HasOne(x => x.Position)
            .WithMany(x => x.PositionKpiHistory)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasOne(x => x.Kpi);
    }
}
