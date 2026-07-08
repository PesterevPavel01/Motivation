using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motivation.AnalyticsCore.Domain.Entities;

namespace Motivation.AnalyticsCore.Infrastructure.Configurations
{
    internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.ToTable("outbox_messages");

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(4096);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.Error)
                .HasMaxLength(8192);

            builder.Property(x => x.ProcessedAt).HasConversion
            (
                src => src != null && src.Value.Kind == DateTimeKind.Utc
                    ? src
                    : DateTime.SpecifyKind(src!.Value, DateTimeKind.Utc),

                dst => dst != null && dst.Value.Kind == DateTimeKind.Utc
                    ? dst
                    : DateTime.SpecifyKind(dst!.Value, DateTimeKind.Utc)
            );

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
        }
    }
}
