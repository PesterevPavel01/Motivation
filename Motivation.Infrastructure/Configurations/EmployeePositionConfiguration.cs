using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;

namespace Motivation.Infrastructure.Configurations;

internal sealed class EmployeePositionConfiguration : IEntityTypeConfiguration<EmployeePosition>
{
    public void Configure(EntityTypeBuilder<EmployeePosition> builder)
    {
        builder.ToTable("employee_positions");

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
            .HasOne(x => x.Employee)
            .WithMany(x => x.EmployeePositions)
            .HasForeignKey(x => x.EmployeeId)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasOne(x => x.Position)
            .WithMany(x => x.EmployeePositions)
            .HasForeignKey(x => x.PositionId)
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
    }
}
