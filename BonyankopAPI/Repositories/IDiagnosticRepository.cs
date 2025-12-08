using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;

namespace BonyankopAPI.Repositories;

public interface IDiagnosticRepository : IRepository<Diagnostic>
{
    Task<IEnumerable<Diagnostic>> GetByCitizenIdAsync(Guid citizenId);
    Task<IEnumerable<Diagnostic>> GetByRiskLevelAsync(RiskLevel riskLevel);
    Task<IEnumerable<Diagnostic>> GetByProblemCategoryAsync(ProblemCategory category);
    Task<IEnumerable<Diagnostic>> GetRecentDiagnosticsAsync(int count = 10);
    Task<Dictionary<ProblemCategory, int>> GetCategoryStatisticsAsync();
    Task<Dictionary<RiskLevel, int>> GetRiskLevelStatisticsAsync();
}
