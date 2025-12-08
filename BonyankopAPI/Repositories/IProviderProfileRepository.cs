using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;

namespace BonyankopAPI.Repositories;

public interface IProviderProfileRepository : IRepository<ProviderProfile>
{
    Task<ProviderProfile?> GetByUserIdAsync(Guid userId);
    Task<bool> ExistsByUserIdAsync(Guid userId);
    Task<IEnumerable<ProviderProfile>> GetByProviderTypeAsync(ProviderType providerType);
    Task<IEnumerable<ProviderProfile>> GetVerifiedProvidersAsync();
    Task<IEnumerable<ProviderProfile>> GetFeaturedProvidersAsync();
    Task<IEnumerable<ProviderProfile>> SearchProvidersAsync(string? searchTerm, ProviderType? providerType, string? coverageArea);
    Task UpdateAverageRatingAsync(Guid providerId, decimal newRating);
    Task UpdateCompletionRateAsync(Guid providerId);
}
