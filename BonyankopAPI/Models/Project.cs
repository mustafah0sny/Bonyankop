using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonyankopAPI.Models;

public class Project
{
    [Key]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid RequestId { get; set; }

    [Required]
    public Guid QuoteId { get; set; }

    [Required]
    public Guid CitizenId { get; set; }

    [Required]
    public Guid ProviderId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProjectTitle { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? ProjectDescription { get; set; }

    [Required]
    public ProjectStatus Status { get; set; } = ProjectStatus.SCHEDULED;

    public DateTime? ScheduledStartDate { get; set; }

    public DateTime? ActualStartDate { get; set; }

    public DateTime? ScheduledEndDate { get; set; }

    public DateTime? ActualCompletionDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AgreedCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualCost { get; set; }

    [MaxLength(1000)]
    public string? CostDifferenceReason { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.PENDING;

    [Required]
    public string WorkNotesJson { get; set; } = "[]";

    [NotMapped]
    public List<WorkNote> WorkNotes
    {
        get => string.IsNullOrEmpty(WorkNotesJson) 
            ? new List<WorkNote>() 
            : System.Text.Json.JsonSerializer.Deserialize<List<WorkNote>>(WorkNotesJson) ?? new List<WorkNote>();
        set => WorkNotesJson = System.Text.Json.JsonSerializer.Serialize(value);
    }

    public List<string> BeforeImages { get; set; } = new();

    public List<string> DuringImages { get; set; } = new();

    public List<string> AfterImages { get; set; } = new();

    [MaxLength(500)]
    public string? TechnicalReportUrl { get; set; }

    [MaxLength(500)]
    public string? CompletionCertificateUrl { get; set; }

    public DateTime? WarrantyStartDate { get; set; }

    public DateTime? WarrantyEndDate { get; set; }

    [MaxLength(1000)]
    public string? CitizenSatisfaction { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(RequestId))]
    public virtual ServiceRequest ServiceRequest { get; set; } = null!;

    [ForeignKey(nameof(QuoteId))]
    public virtual Quote Quote { get; set; } = null!;

    [ForeignKey(nameof(CitizenId))]
    public virtual User Citizen { get; set; } = null!;

    [ForeignKey(nameof(ProviderId))]
    public virtual ProviderProfile Provider { get; set; } = null!;

    // Business methods
    public int? CalculateDuration()
    {
        if (ActualStartDate.HasValue && ActualCompletionDate.HasValue)
        {
            return (ActualCompletionDate.Value - ActualStartDate.Value).Days;
        }
        return null;
    }

    public bool IsOverdue()
    {
        if (ScheduledEndDate.HasValue && Status == ProjectStatus.IN_PROGRESS)
        {
            return DateTime.UtcNow > ScheduledEndDate.Value;
        }
        return false;
    }
}
