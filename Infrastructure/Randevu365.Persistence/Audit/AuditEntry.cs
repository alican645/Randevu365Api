using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Audit;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public int? UserId { get; set; }
    public string TableName { get; set; } = null!;
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = new();

    public AuditLog ToAuditLog()
    {
        return new AuditLog
        {
            UserId = UserId,
            Type = AuditType.ToString(),
            TableName = TableName,
            DateTime = DateTime.UtcNow,
            PrimaryKey = JsonSerializer.Serialize(KeyValues),
            OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
            NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
            AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns)
        };
    }
}

public enum AuditType
{
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 3
}
