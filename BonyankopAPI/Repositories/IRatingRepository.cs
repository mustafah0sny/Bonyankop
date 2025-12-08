using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;

namespace BonyankopAPI.Repositories;

public interface IRatingRepository : IRepository<Rating>
{
    Task<IEnumerable<Rating>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<Rating>> GetByProviderIdAsync(Guid providerId);
    Task<IEnumerable<Rating>> GetByCitizenIdAsync(Guid citizenId);
    Task<Rating?> GetByProjectAndCitizenAsync(Guid projectId, Guid citizenId);
    Task<IEnumerable<Rating>> GetFeaturedRatingsAsync(int count = 10);
    Task<decimal> GetProviderAverageRatingAsync(Guid providerId);
}
