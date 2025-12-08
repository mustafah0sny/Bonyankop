namespace BonyankopAPI.DTOs;

public class AuditLogResponseDto
{
    public Guid LogId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? ActionDescription { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestUrl { get; set; }
    public int? ResponseStatus { get; set; }
    public int? ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SessionId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuditLogSearchDto
{
    public string? ActionType { get; set; }
    public string? EntityType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Limit { get; set; } = 100;
}
