using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs;

public class CreateQuoteDto
{
    [Required(ErrorMessage = "Request ID is required")]
    public Guid RequestId { get; set; }

    [Required(ErrorMessage = "Estimated cost is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Estimated cost must be positive")]
    public decimal EstimatedCost { get; set; }

    [Required(ErrorMessage = "Cost breakdown is required")]
    public CostBreakdownDto CostBreakdown { get; set; } = null!;

    [Required(ErrorMessage = "Estimated duration is required")]
    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    public int EstimatedDurationDays { get; set; }

    [Required(ErrorMessage = "Technical assessment is required")]
    [StringLength(2000, ErrorMessage = "Technical assessment cannot exceed 2000 characters")]
    public string TechnicalAssessment { get; set; } = string.Empty;

    [Required(ErrorMessage = "Proposed solution is required")]
    [StringLength(2000, ErrorMessage = "Proposed solution cannot exceed 2000 characters")]
    public string ProposedSolution { get; set; } = string.Empty;

    public bool MaterialsIncluded { get; set; } = false;

    [Range(0, 120, ErrorMessage = "Warranty period must be between 0 and 120 months")]
    public int WarrantyPeriodMonths { get; set; }

    [StringLength(2000, ErrorMessage = "Terms cannot exceed 2000 characters")]
    public string? TermsAndConditions { get; set; }

    [Range(1, 90, ErrorMessage = "Validity period must be between 1 and 90 days")]
    public int ValidityPeriodDays { get; set; } = 30;

    public List<string> Attachments { get; set; } = new();
}
