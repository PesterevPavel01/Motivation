using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.Domain.Entities.Base;
using Motivation.Domain.ValueObjects;

namespace Motivation.Infrastructure.Configurations.Base
{
    public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
     where TEntity : Entity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToTable(TableName());

            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasIndex(x => x.Code)
                .IsUnique();

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
                .Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(CodeValue.MaxCodeLength)
                .HasConversion(x => x.Value, x => CodeValue.Create(x).Result);

            AddBuilder(builder);
        }

        protected abstract void AddBuilder(EntityTypeBuilder<TEntity> builder);

        protected abstract string TableName();
    }
}
