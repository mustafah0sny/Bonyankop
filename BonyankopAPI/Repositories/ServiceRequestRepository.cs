using BonyankopAPI.Data;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BonyankopAPI.Repositories;

public class ServiceRequestRepository : Repository<ServiceRequest>, IServiceRequestRepository
{
    public ServiceRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ServiceRequest>> GetByCitizenIdAsync(Guid citizenId)
    {
        return await _context.Set<ServiceRequest>()
            .Where(sr => sr.CitizenId == citizenId)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> GetByStatusAsync(RequestStatus status)
    {
        return await _context.Set<ServiceRequest>()
            .Where(sr => sr.Status == status)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> GetByCategoryAsync(string category)
    {
        return await _context.Set<ServiceRequest>()
            .Where(sr => sr.ProblemCategory == category)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> GetActiveRequestsAsync()
    {
        return await _context.Set<ServiceRequest>()
            .Where(sr => sr.Status == RequestStatus.OPEN || sr.Status == RequestStatus.QUOTES_RECEIVED)
            .Where(sr => sr.ExpiresAt == null || sr.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> GetExpiringSoonAsync(int days = 3)
    {
        var threshold = DateTime.UtcNow.AddDays(days);
        return await _context.Set<ServiceRequest>()
            .Where(sr => sr.Status == RequestStatus.OPEN || sr.Status == RequestStatus.QUOTES_RECEIVED)
            .Where(sr => sr.ExpiresAt != null && sr.ExpiresAt <= threshold && sr.ExpiresAt > DateTime.UtcNow)
            .OrderBy(sr => sr.ExpiresAt)
            .ToListAsync();
    }

    public async Task IncrementViewsAsync(Guid requestId)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            request.ViewsCount++;
            Update(request);
            await SaveChangesAsync();
        }
    }

    public async Task UpdateQuotesCountAsync(Guid requestId)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            var quotesCount = await _context.Set<Quote>()
                .CountAsync(q => q.RequestId == requestId);
            
            request.QuotesCount = quotesCount;
            
            if (quotesCount > 0 && request.Status == RequestStatus.OPEN)
            {
                request.Status = RequestStatus.QUOTES_RECEIVED;
            }
            
            Update(request);
            await SaveChangesAsync();
        }
    }
}
