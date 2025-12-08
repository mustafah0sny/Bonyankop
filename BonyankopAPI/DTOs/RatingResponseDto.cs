namespace BonyankopAPI.DTOs;

public class RatingResponseDto
{
    public Guid RatingId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid CitizenId { get; set; }
    public string CitizenName { get; set; } = string.Empty;
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public int OverallRating { get; set; }
    public int QualityRating { get; set; }
    public int TimelinessRating { get; set; }
    public int ProfessionalismRating { get; set; }
    public int ValueRating { get; set; }
    public int CommunicationRating { get; set; }
    public string? ReviewTitle { get; set; }
    public string? ReviewText { get; set; }
    public bool? WouldRecommend { get; set; }
    public string? ResponseFromProvider { get; set; }
    public DateTime? ResponseAt { get; set; }
    public bool IsVerified { get; set; }
    public bool IsFeatured { get; set; }
    public int HelpfulCount { get; set; }
    public decimal AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
