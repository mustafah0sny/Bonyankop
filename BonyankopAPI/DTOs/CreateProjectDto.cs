using System.ComponentModel.DataAnnotations;
using BonyankopAPI.Models;

namespace BonyankopAPI.DTOs;

public class CreateProjectDto
{
    [Required(ErrorMessage = "Quote ID is required")]
    public Guid QuoteId { get; set; }

    [StringLength(200, ErrorMessage = "Project title cannot exceed 200 characters")]
    public string? ProjectTitle { get; set; }

    [StringLength(2000, ErrorMessage = "Project description cannot exceed 2000 characters")]
    public string? ProjectDescription { get; set; }

    public DateTime? ScheduledStartDate { get; set; }

    public DateTime? ScheduledEndDate { get; set; }
}
