using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;

namespace BonyankopAPI.Repositories;

public interface IQuoteRepository : IRepository<Quote>
{
    Task<IEnumerable<Quote>> GetByRequestIdAsync(Guid requestId);
    Task<IEnumerable<Quote>> GetByProviderIdAsync(Guid providerId);
    Task<IEnumerable<Quote>> GetByStatusAsync(QuoteStatus status);
    Task<Quote?> GetByRequestAndProviderAsync(Guid requestId, Guid providerId);
    Task<IEnumerable<Quote>> GetExpiredQuotesAsync();
    Task MarkExpiredQuotesAsync();
}
