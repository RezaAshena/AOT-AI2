using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AOT.Domain.Entities;

namespace AOT.Infrastructure.Persistence.Configurations;

public class WorkflowInstanceConfiguration : IEntityTypeConfiguration<WorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        builder.ToTable("WorkflowInstances");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.RowVersion)
            .IsRowVersion();

        builder.Property(w => w.WorkflowName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.WorkflowDefinitionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(w => w.Context)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        builder.Property(w => w.Error)
            .HasMaxLength(2000);

        builder.HasMany(w => w.Steps)
            .WithOne()
            .HasForeignKey(s => s.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.CreatedAt);
    }
}
