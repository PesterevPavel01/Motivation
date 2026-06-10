using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Motivation.Infrastructure.Configurations.Base;

namespace Motivation.Infrastructure.Configurations;

internal sealed class EmployeeConfiguration : EntityConfiguration<Employee>
{
    protected override void AddBuilder(EntityTypeBuilder<Employee> builder)
    {
        builder
            .Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(FirstNameValue.MaxFirstNameLength)
            .HasConversion(x => x.Value, x => FirstNameValue.Create(x).Result);

        builder
            .Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(LastNameValue.MaxLastNameLength)
            .HasConversion(x => x.Value, x => LastNameValue.Create(x).Result);

        builder
            .Property(x => x.SecondName)
            .IsRequired(false)
            .HasMaxLength(SecondNameValue.MaxSecondNameLength)
            .HasConversion(
                x => x != null ? x.Value : null,
                x => x != null ? SecondNameValue.Create(x).Result : null);

        builder
            .Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(FullNameValue.MaxFullNameLength)
            .HasConversion(x => x.Value, x => FullNameValue.Create(x).Result);

        builder
            .HasMany(x => x.EmployeePositions)
            .WithOne(x => x.Employee)
            .HasPrincipalKey(x => x.Id);

        builder
            .HasIndex(x => x.FullName);

        builder
            .HasIndex(x => new { x.FirstName, x.LastName});
    }

    protected override string TableName()
     => "employees";
}
