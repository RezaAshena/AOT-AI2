namespace AOT.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    protected void MarkAsModified(string? modifiedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = modifiedBy;
    }
}
