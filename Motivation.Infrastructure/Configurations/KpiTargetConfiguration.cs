using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;

namespace Motivation.Infrastructure.Configurations
{
    internal class KpiTargetConfiguration : IEntityTypeConfiguration<KpiTarget>
    {
        public void Configure(EntityTypeBuilder<KpiTarget> builder)
        {
            builder.ToTable("kpi_targets");

            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.Target)
                .IsRequired()
                .HasConversion(x => x.Value, x => TargetValue.Create(x).Result);

            builder.OwnsOne(x => x.KpiFact, kpiFactBuilder =>
            {
                kpiFactBuilder.ToJson();

                kpiFactBuilder
                    .Property(x => x.Fact)
                    .IsRequired()
                    .HasConversion(x => x.Value, x => FactValue.Create(x).Result);

                kpiFactBuilder
                    .Property(x => x.Achievement)
                    .IsRequired()
                    .HasConversion(x => x.Value, x => AchievementValue.Create(x).Result);

                kpiFactBuilder
                    .Property(x => x.BonusAmount)
                    .IsRequired()
                    .HasConversion(x => x.Value, x => BonusAmountValue.Create(x).Result);
            });
        }
    }
}
