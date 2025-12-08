using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;

namespace BonyankopAPI.Repositories;

public interface IPortfolioItemRepository : IRepository<PortfolioItem>
{
    Task<IEnumerable<PortfolioItem>> GetByProviderIdAsync(Guid providerId);
    Task<IEnumerable<PortfolioItem>> GetFeaturedByProviderIdAsync(Guid providerId);
    Task<PortfolioItem?> GetByIdAndProviderIdAsync(Guid portfolioId, Guid providerId);
    Task<int> GetMaxDisplayOrderAsync(Guid providerId);
    Task ReorderAsync(Guid providerId, Guid portfolioId, int newOrder);
}
