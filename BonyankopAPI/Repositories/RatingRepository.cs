using BonyankopAPI.Data;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BonyankopAPI.Repositories;

public class RatingRepository : Repository<Rating>, IRatingRepository
{
    public RatingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rating>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Set<Rating>()
            .Where(r => r.ProjectId == projectId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rating>> GetByProviderIdAsync(Guid providerId)
    {
        return await _context.Set<Rating>()
            .Where(r => r.ProviderId == providerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rating>> GetByCitizenIdAsync(Guid citizenId)
    {
        return await _context.Set<Rating>()
            .Where(r => r.CitizenId == citizenId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Rating?> GetByProjectAndCitizenAsync(Guid projectId, Guid citizenId)
    {
        return await _context.Set<Rating>()
            .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.CitizenId == citizenId);
    }

    public async Task<IEnumerable<Rating>> GetFeaturedRatingsAsync(int count = 10)
    {
        return await _context.Set<Rating>()
            .Where(r => r.IsFeatured)
            .OrderByDescending(r => r.OverallRating)
            .ThenByDescending(r => r.HelpfulCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<decimal> GetProviderAverageRatingAsync(Guid providerId)
    {
        var ratings = await _context.Set<Rating>()
            .Where(r => r.ProviderId == providerId)
            .ToListAsync();

        if (!ratings.Any())
            return 0;

        return (decimal)ratings.Average(r => r.OverallRating);
    }
}
