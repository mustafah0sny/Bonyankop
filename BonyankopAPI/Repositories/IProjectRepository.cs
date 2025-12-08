using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;

namespace BonyankopAPI.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByCitizenIdAsync(Guid citizenId);
    Task<IEnumerable<Project>> GetByProviderIdAsync(Guid providerId);
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status);
    Task<Project?> GetByQuoteIdAsync(Guid quoteId);
    Task<IEnumerable<Project>> GetOverdueProjectsAsync();
    Task<IEnumerable<Project>> GetActiveProjectsAsync();
}
