using AOT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AOT.Infrastructure.Persistence.Configurations;

public class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
{
    public void Configure(EntityTypeBuilder<NotificationMessage> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.RowVersion)
            .IsRowVersion();

        builder.Property(n => n.RecipientId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Subject)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(n => n.Body)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(n => n.Channel)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(n => n.Priority)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(n => n.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Error)
            .HasMaxLength(2000);

        builder.HasIndex(n => n.IdempotencyKey).IsUnique();
        builder.HasIndex(n => new { n.IsSent, n.CreatedAt });
        builder.HasIndex(n => n.RecipientId);
    }
}
