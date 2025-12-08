using BonyankopAPI.Models;

namespace BonyankopAPI.DTOs;

public class ProjectResponseDto
{
    public Guid ProjectId { get; set; }
    public Guid RequestId { get; set; }
    public Guid QuoteId { get; set; }
    public Guid CitizenId { get; set; }
    public string CitizenName { get; set; } = string.Empty;
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string ProjectTitle { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ScheduledEndDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }
    public decimal AgreedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public string? CostDifferenceReason { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public List<WorkNoteDto> WorkNotes { get; set; } = new();
    public List<string> BeforeImages { get; set; } = new();
    public List<string> DuringImages { get; set; } = new();
    public List<string> AfterImages { get; set; } = new();
    public string? TechnicalReportUrl { get; set; }
    public string? CompletionCertificateUrl { get; set; }
    public DateTime? WarrantyStartDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public string? CitizenSatisfaction { get; set; }
    public int? Duration { get; set; }
    public bool IsOverdue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
