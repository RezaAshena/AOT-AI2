using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AOT.Domain.Aggregates;

namespace AOT.Infrastructure.Persistence.Configurations;

public class ApprovalRequestConfiguration : IEntityTypeConfiguration<ApprovalRequest>
{
    public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
    {
        builder.ToTable("ApprovalRequests");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.RequesterId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.ApproverId)
            .HasMaxLength(200);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.ApprovalComments)
            .HasMaxLength(1000);

        builder.Property(a => a.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        builder.Ignore(a => a.DomainEvents);

        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.RequesterId);
        builder.HasIndex(a => a.WorkflowInstanceId);
        builder.HasIndex(a => a.ExpiresAt);
    }
}
