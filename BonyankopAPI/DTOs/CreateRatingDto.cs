using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs;

public class CreateRatingDto
{
    [Required(ErrorMessage = "Project ID is required")]
    public Guid ProjectId { get; set; }

    [Required(ErrorMessage = "Overall rating is required")]
    [Range(1, 5, ErrorMessage = "Overall rating must be between 1 and 5")]
    public int OverallRating { get; set; }

    [Required(ErrorMessage = "Quality rating is required")]
    [Range(1, 5, ErrorMessage = "Quality rating must be between 1 and 5")]
    public int QualityRating { get; set; }

    [Required(ErrorMessage = "Timeliness rating is required")]
    [Range(1, 5, ErrorMessage = "Timeliness rating must be between 1 and 5")]
    public int TimelinessRating { get; set; }

    [Required(ErrorMessage = "Professionalism rating is required")]
    [Range(1, 5, ErrorMessage = "Professionalism rating must be between 1 and 5")]
    public int ProfessionalismRating { get; set; }

    [Required(ErrorMessage = "Value rating is required")]
    [Range(1, 5, ErrorMessage = "Value rating must be between 1 and 5")]
    public int ValueRating { get; set; }

    [Required(ErrorMessage = "Communication rating is required")]
    [Range(1, 5, ErrorMessage = "Communication rating must be between 1 and 5")]
    public int CommunicationRating { get; set; }

    [StringLength(200, ErrorMessage = "Review title cannot exceed 200 characters")]
    public string? ReviewTitle { get; set; }

    [StringLength(2000, ErrorMessage = "Review text cannot exceed 2000 characters")]
    public string? ReviewText { get; set; }

    public bool? WouldRecommend { get; set; }
}
