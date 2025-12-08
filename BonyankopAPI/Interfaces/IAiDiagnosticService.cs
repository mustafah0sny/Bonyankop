using BonyankopAPI.Models;

namespace BonyankopAPI.Interfaces;

public interface IAiDiagnosticService
{
    /// <summary>
    /// Analyze an image and generate diagnostic results
    /// </summary>
    Task<DiagnosticResult> AnalyzeImageAsync(string imageUrl, ImageMetadata metadata);

    /// <summary>
    /// Get recommendations based on diagnostic results
    /// </summary>
    List<string> GetRecommendations(ProblemCategory category, RiskLevel riskLevel, bool isDiyPossible);
}

public class DiagnosticResult
{
    public RiskLevel RiskLevel { get; set; }
    public ProblemCategory ProblemCategory { get; set; }
    public string ProblemSubcategory { get; set; } = string.Empty;
    public string ProbableCause { get; set; } = string.Empty;
    public string RiskPrediction { get; set; } = string.Empty;
    public string RecommendedAction { get; set; } = string.Empty;
    public decimal AiConfidenceScore { get; set; }
    public bool IsDiyPossible { get; set; }
    public string EstimatedCostRange { get; set; } = string.Empty;
    public UrgencyLevel UrgencyLevel { get; set; }
    public int ProcessingTimeMs { get; set; }
}
