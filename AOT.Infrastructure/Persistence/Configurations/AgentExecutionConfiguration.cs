using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AOT.Domain.Entities;

namespace AOT.Infrastructure.Persistence.Configurations;

public class StepExecutionConfiguration : IEntityTypeConfiguration<StepExecution>
{
    public void Configure(EntityTypeBuilder<StepExecution> builder)
    {
        builder.ToTable("StepExecutions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.RowVersion)
            .IsRowVersion();

        builder.Property(s => s.StepName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.Input)
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.Output)
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.Error)
            .HasMaxLength(2000);

        builder.HasIndex(s => s.WorkflowInstanceId);
        builder.HasIndex(s => s.Status);
    }
}

public class AgentExecutionConfiguration : IEntityTypeConfiguration<AgentExecution>
{
    public void Configure(EntityTypeBuilder<AgentExecution> builder)
    {
        builder.ToTable("AgentExecutions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        builder.Property(a => a.AgentName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.AgentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.Request)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.Response)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.Error)
            .HasMaxLength(2000);

        builder.HasIndex(a => a.WorkflowInstanceId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.CreatedAt);
    }
}
