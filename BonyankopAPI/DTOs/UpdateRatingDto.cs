using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs;

public class UpdateRatingDto
{
    [Range(1, 5, ErrorMessage = "Overall rating must be between 1 and 5")]
    public int? OverallRating { get; set; }

    [Range(1, 5, ErrorMessage = "Quality rating must be between 1 and 5")]
    public int? QualityRating { get; set; }

    [Range(1, 5, ErrorMessage = "Timeliness rating must be between 1 and 5")]
    public int? TimelinessRating { get; set; }

    [Range(1, 5, ErrorMessage = "Professionalism rating must be between 1 and 5")]
    public int? ProfessionalismRating { get; set; }

    [Range(1, 5, ErrorMessage = "Value rating must be between 1 and 5")]
    public int? ValueRating { get; set; }

    [Range(1, 5, ErrorMessage = "Communication rating must be between 1 and 5")]
    public int? CommunicationRating { get; set; }

    [StringLength(200, ErrorMessage = "Review title cannot exceed 200 characters")]
    public string? ReviewTitle { get; set; }

    [StringLength(2000, ErrorMessage = "Review text cannot exceed 2000 characters")]
    public string? ReviewText { get; set; }

    public bool? WouldRecommend { get; set; }
}
