using BonyankopAPI.Data;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BonyankopAPI.Repositories;

public class DiagnosticRepository : Repository<Diagnostic>, IDiagnosticRepository
{
    public DiagnosticRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Diagnostic>> GetByCitizenIdAsync(Guid citizenId)
    {
        return await _context.Set<Diagnostic>()
            .Where(d => d.CitizenId == citizenId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Diagnostic>> GetByRiskLevelAsync(RiskLevel riskLevel)
    {
        return await _context.Set<Diagnostic>()
            .Where(d => d.RiskLevel == riskLevel)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Diagnostic>> GetByProblemCategoryAsync(ProblemCategory category)
    {
        return await _context.Set<Diagnostic>()
            .Where(d => d.ProblemCategory == category)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Diagnostic>> GetRecentDiagnosticsAsync(int count = 10)
    {
        return await _context.Set<Diagnostic>()
            .OrderByDescending(d => d.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<ProblemCategory, int>> GetCategoryStatisticsAsync()
    {
        return await _context.Set<Diagnostic>()
            .GroupBy(d => d.ProblemCategory)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count);
    }

    public async Task<Dictionary<RiskLevel, int>> GetRiskLevelStatisticsAsync()
    {
        return await _context.Set<Diagnostic>()
            .GroupBy(d => d.RiskLevel)
            .Select(g => new { RiskLevel = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RiskLevel, x => x.Count);
    }
}
