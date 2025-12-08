using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonyankopAPI.Models;

public class Quote
{
    [Key]
    public Guid QuoteId { get; set; }

    [Required]
    public Guid RequestId { get; set; }

    [Required]
    public Guid ProviderId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedCost { get; set; }

    [Required]
    public string CostBreakdownJson { get; set; } = string.Empty;

    [NotMapped]
    public CostBreakdown CostBreakdown
    {
        get => string.IsNullOrEmpty(CostBreakdownJson) 
            ? new CostBreakdown() 
            : System.Text.Json.JsonSerializer.Deserialize<CostBreakdown>(CostBreakdownJson) ?? new CostBreakdown();
        set => CostBreakdownJson = System.Text.Json.JsonSerializer.Serialize(value);
    }

    public int? EstimatedDurationDays { get; set; }

    [MaxLength(2000)]
    public string? TechnicalAssessment { get; set; }

    [MaxLength(2000)]
    public string? ProposedSolution { get; set; }

    public bool MaterialsIncluded { get; set; } = false;

    public int? WarrantyPeriodMonths { get; set; }

    [MaxLength(2000)]
    public string? TermsAndConditions { get; set; }

    public int ValidityPeriodDays { get; set; } = 30;

    public List<string> Attachments { get; set; } = new();

    [Required]
    public QuoteStatus Status { get; set; } = QuoteStatus.PENDING;

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(RequestId))]
    public virtual ServiceRequest ServiceRequest { get; set; } = null!;

    [ForeignKey(nameof(ProviderId))]
    public virtual ProviderProfile Provider { get; set; } = null!;

    public bool IsExpired()
    {
        return Status == QuoteStatus.PENDING && DateTime.UtcNow > ExpiresAt;
    }
}
