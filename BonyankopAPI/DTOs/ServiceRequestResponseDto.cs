using BonyankopAPI.Models;

namespace BonyankopAPI.DTOs;

public class ServiceRequestResponseDto
{
    public Guid RequestId { get; set; }
    public Guid? DiagnosticId { get; set; }
    public Guid CitizenId { get; set; }
    public string CitizenName { get; set; } = string.Empty;
    public string ProblemTitle { get; set; } = string.Empty;
    public string ProblemDescription { get; set; } = string.Empty;
    public string ProblemCategory { get; set; } = string.Empty;
    public List<string> AdditionalImages { get; set; } = new();
    public string? PreferredProviderType { get; set; }
    public DateTime? PreferredServiceDate { get; set; }
    public string? PropertyType { get; set; }
    public string? PropertyAddress { get; set; }
    public string? ContactPhone { get; set; }
    public RequestStatus Status { get; set; }
    public int QuotesCount { get; set; }
    public int ViewsCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
