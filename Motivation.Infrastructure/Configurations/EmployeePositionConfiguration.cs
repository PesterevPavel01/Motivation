using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.Infrastructure.Configurations;

internal sealed class EmployeePositionConfiguration : IEntityTypeConfiguration<EmployeePosition>
{
    public void Configure(EntityTypeBuilder<EmployeePosition> builder)
    {
        builder.ToTable("employee_positions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

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

        builder.Property(x => x.AssignmentDate).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );

        builder
            .HasOne(x => x.Employee)
            .WithMany(x => x.EmployeePositions)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasOne(x => x.Position)
            .WithMany(x => x.EmployeePositions)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasMany(x => x.ExtraPartHistory)
            .WithOne(x => x.EmployeePosition);

        builder
            .HasMany(x => x.DeductionHistory)
            .WithOne(x => x.EmployeePosition);

        builder
            .HasIndex(x => new { x.PositionId, x.EmployeeId })
            .IsUnique();

        builder.OwnsMany(x => x.ActualHoursHistory, actualHoursBuilder =>
        {
            actualHoursBuilder.ToJson();

            actualHoursBuilder
                .Property(x => x.Hours)
                .IsRequired()
                .HasConversion(x => x.Value, x => HoursValue.Create(x).Result);

            actualHoursBuilder.Property(x => x.Month)
                .IsRequired()
                .HasConversion(x => x.Value, x => MonthValue.Create(x).Result);

            actualHoursBuilder.Property(x => x.Year)
                .IsRequired()
                .HasConversion(x => x.Value, x => YearValue.Create(x).Result);

            actualHoursBuilder.Property(x => x.CreatedAt).IsRequired().HasConversion
            (
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            );

            actualHoursBuilder.Property(x => x.UpdatedAt).IsRequired().HasConversion
            (
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            );

            actualHoursBuilder
                .HasIndex(x => new { x.Month, x.Year})
                .IsUnique();
        });
    }
}
