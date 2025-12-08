using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs;

public class UpdateQuoteDto
{
    [Range(0, double.MaxValue, ErrorMessage = "Estimated cost must be positive")]
    public decimal? EstimatedCost { get; set; }

    public CostBreakdownDto? CostBreakdown { get; set; }

    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    public int? EstimatedDurationDays { get; set; }

    [StringLength(2000, ErrorMessage = "Technical assessment cannot exceed 2000 characters")]
    public string? TechnicalAssessment { get; set; }

    [StringLength(2000, ErrorMessage = "Proposed solution cannot exceed 2000 characters")]
    public string? ProposedSolution { get; set; }

    public bool? MaterialsIncluded { get; set; }

    [Range(0, 120, ErrorMessage = "Warranty period must be between 0 and 120 months")]
    public int? WarrantyPeriodMonths { get; set; }

    [StringLength(2000, ErrorMessage = "Terms cannot exceed 2000 characters")]
    public string? TermsAndConditions { get; set; }

    [Range(1, 90, ErrorMessage = "Validity period must be between 1 and 90 days")]
    public int? ValidityPeriodDays { get; set; }

    public List<string>? Attachments { get; set; }
}
