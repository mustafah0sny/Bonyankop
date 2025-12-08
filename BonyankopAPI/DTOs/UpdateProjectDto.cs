using System.ComponentModel.DataAnnotations;
using BonyankopAPI.Models;

namespace BonyankopAPI.DTOs;

public class UpdateProjectDto
{
    public ProjectStatus? Status { get; set; }

    public DateTime? ScheduledStartDate { get; set; }

    public DateTime? ActualStartDate { get; set; }

    public DateTime? ScheduledEndDate { get; set; }

    public DateTime? ActualCompletionDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Actual cost must be positive")]
    public decimal? ActualCost { get; set; }

    [StringLength(1000, ErrorMessage = "Cost difference reason cannot exceed 1000 characters")]
    public string? CostDifferenceReason { get; set; }

    public PaymentStatus? PaymentStatus { get; set; }

    [StringLength(500, ErrorMessage = "Technical report URL cannot exceed 500 characters")]
    public string? TechnicalReportUrl { get; set; }

    [StringLength(500, ErrorMessage = "Completion certificate URL cannot exceed 500 characters")]
    public string? CompletionCertificateUrl { get; set; }

    public DateTime? WarrantyStartDate { get; set; }

    public DateTime? WarrantyEndDate { get; set; }

    [StringLength(1000, ErrorMessage = "Citizen satisfaction cannot exceed 1000 characters")]
    public string? CitizenSatisfaction { get; set; }
}
