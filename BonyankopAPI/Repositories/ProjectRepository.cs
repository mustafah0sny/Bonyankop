using BonyankopAPI.Data;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BonyankopAPI.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetByCitizenIdAsync(Guid citizenId)
    {
        return await _context.Set<Project>()
            .Where(p => p.CitizenId == citizenId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByProviderIdAsync(Guid providerId)
    {
        return await _context.Set<Project>()
            .Where(p => p.ProviderId == providerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status)
    {
        return await _context.Set<Project>()
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetByQuoteIdAsync(Guid quoteId)
    {
        return await _context.Set<Project>()
            .FirstOrDefaultAsync(p => p.QuoteId == quoteId);
    }

    public async Task<IEnumerable<Project>> GetOverdueProjectsAsync()
    {
        return await _context.Set<Project>()
            .Where(p => p.Status == ProjectStatus.IN_PROGRESS && 
                       p.ScheduledEndDate != null && 
                       p.ScheduledEndDate < DateTime.UtcNow)
            .OrderBy(p => p.ScheduledEndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
    {
        return await _context.Set<Project>()
            .Where(p => p.Status == ProjectStatus.SCHEDULED || p.Status == ProjectStatus.IN_PROGRESS)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}
