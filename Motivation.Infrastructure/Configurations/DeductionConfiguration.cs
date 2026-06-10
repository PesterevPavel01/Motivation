using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Motivation.Infrastructure.Configurations.Base;

namespace Motivation.Infrastructure.Configurations
{
    public sealed class DeductionConfiguration : EntityConfiguration<Deduction>
    {
        protected override void AddBuilder(EntityTypeBuilder<Deduction> builder)
        {
            builder
                .Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(TitleValue.MaxTitleLength)
                .HasConversion(x => x.Value, x => TitleValue.Create(x).Result);

            builder
                .Property(x => x.DeductionValue)
                .IsRequired()
                .HasConversion(x => x.Value, x => DeductionValue.Create(x).Result);

            builder
                .Property(x => x.Month)
                .IsRequired()
                .HasConversion(x => x.Value, x => MonthValue.Create(x).Result);

            builder
                .Property(x => x.Year)
                .IsRequired()
                .HasConversion(x => x.Value, x => YearValue.Create(x).Result);
        }

        protected override string TableName()
            => "deductions";
    }
}
