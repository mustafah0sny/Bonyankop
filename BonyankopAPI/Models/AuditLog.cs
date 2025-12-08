namespace BonyankopAPI.Models;

public class AuditLog
{
    public Guid LogId { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? ActionDescription { get; set; }
    public string? OldValuesJson { get; set; } // Stored as JSON string
    public string? NewValuesJson { get; set; } // Stored as JSON string
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestUrl { get; set; }
    public int? ResponseStatus { get; set; }
    public int? ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SessionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
}
