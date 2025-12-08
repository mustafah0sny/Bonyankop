using BonyankopAPI.Models;

namespace BonyankopAPI.DTOs;

public class QuoteResponseDto
{
    public Guid QuoteId { get; set; }
    public Guid RequestId { get; set; }
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public CostBreakdownDto CostBreakdown { get; set; } = null!;
    public int? EstimatedDurationDays { get; set; }
    public string? TechnicalAssessment { get; set; }
    public string? ProposedSolution { get; set; }
    public bool MaterialsIncluded { get; set; }
    public int? WarrantyPeriodMonths { get; set; }
    public string? TermsAndConditions { get; set; }
    public int ValidityPeriodDays { get; set; }
    public List<string> Attachments { get; set; } = new();
    public QuoteStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsExpired { get; set; }
}
