using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.Infrastructure.Configurations
{
    internal sealed class StandardHoursConfiguration : IEntityTypeConfiguration<StandardHours>
    {
        public void Configure(EntityTypeBuilder<StandardHours> builder)
        {
            builder.ToTable("standard_hours");

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

            builder
                .Property(x => x.WorkWeekType)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion<string>();

            builder
                .Property(x => x.StandardHoursValue)
                .IsRequired()
                .HasConversion(x => x.Value, x => StandardHoursValue.Create(x).Result);

            builder
                .Property(x => x.Month)
                .IsRequired()
                .HasConversion(x => x.Value, x => MonthValue.Create(x).Result);

            builder
                .Property(x => x.Year)
                .IsRequired()
                .HasConversion(x => x.Value, x => YearValue.Create(x).Result);
        }
    }
}
