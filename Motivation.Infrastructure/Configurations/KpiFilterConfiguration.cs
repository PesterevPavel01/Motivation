using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Infrastructure.Configurations;

internal class KpiFilterConfiguration : IEntityTypeConfiguration<KpiFilter>
{
    public void Configure(EntityTypeBuilder<KpiFilter> builder)
    {
        builder.ToTable("kpi_filters");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(TitleValue.MaxTitleLength)
            .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

        builder.HasIndex(x => x.Title);

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

        builder.HasDiscriminator(x => x.KpiFilterType)
            .HasValue<KpiFilterCompany>(KpiFilterType.Company)
            .HasValue<KpiFilterProduct>(KpiFilterType.Product)
            .HasValue<KpiFilterCustomer>(KpiFilterType.Customer);
    }
}
