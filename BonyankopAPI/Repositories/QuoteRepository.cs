using BonyankopAPI.Data;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BonyankopAPI.Repositories;

public class QuoteRepository : Repository<Quote>, IQuoteRepository
{
    public QuoteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Quote>> GetByRequestIdAsync(Guid requestId)
    {
        return await _context.Set<Quote>()
            .Where(q => q.RequestId == requestId)
            .OrderByDescending(q => q.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Quote>> GetByProviderIdAsync(Guid providerId)
    {
        return await _context.Set<Quote>()
            .Where(q => q.ProviderId == providerId)
            .OrderByDescending(q => q.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Quote>> GetByStatusAsync(QuoteStatus status)
    {
        return await _context.Set<Quote>()
            .Where(q => q.Status == status)
            .OrderByDescending(q => q.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Quote?> GetByRequestAndProviderAsync(Guid requestId, Guid providerId)
    {
        return await _context.Set<Quote>()
            .FirstOrDefaultAsync(q => q.RequestId == requestId && q.ProviderId == providerId);
    }

    public async Task<IEnumerable<Quote>> GetExpiredQuotesAsync()
    {
        return await _context.Set<Quote>()
            .Where(q => q.Status == QuoteStatus.PENDING && q.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task MarkExpiredQuotesAsync()
    {
        var expiredQuotes = await GetExpiredQuotesAsync();
        foreach (var quote in expiredQuotes)
        {
            quote.Status = QuoteStatus.EXPIRED;
            Update(quote);
        }
        await SaveChangesAsync();
    }
}
