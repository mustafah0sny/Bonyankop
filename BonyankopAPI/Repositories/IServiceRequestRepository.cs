using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;

namespace BonyankopAPI.Repositories;

public interface IServiceRequestRepository : IRepository<ServiceRequest>
{
    Task<IEnumerable<ServiceRequest>> GetByCitizenIdAsync(Guid citizenId);
    Task<IEnumerable<ServiceRequest>> GetByStatusAsync(RequestStatus status);
    Task<IEnumerable<ServiceRequest>> GetByCategoryAsync(string category);
    Task<IEnumerable<ServiceRequest>> GetActiveRequestsAsync();
    Task<IEnumerable<ServiceRequest>> GetExpiringSoonAsync(int days = 3);
    Task IncrementViewsAsync(Guid requestId);
    Task UpdateQuotesCountAsync(Guid requestId);
}
