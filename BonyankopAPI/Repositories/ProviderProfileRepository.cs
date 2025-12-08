using BonyankopAPI.Data;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BonyankopAPI.Repositories;

public class ProviderProfileRepository : Repository<ProviderProfile>, IProviderProfileRepository
{
    public ProviderProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProviderProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Set<ProviderProfile>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId)
    {
        return await _context.Set<ProviderProfile>().AnyAsync(p => p.UserId == userId);
    }

    public async Task<IEnumerable<ProviderProfile>> GetByProviderTypeAsync(ProviderType providerType)
    {
        return await _context.Set<ProviderProfile>()
            .Include(p => p.User)
            .Where(p => p.ProviderType == providerType)
            .OrderByDescending(p => p.AverageRating)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProviderProfile>> GetVerifiedProvidersAsync()
    {
        return await _context.Set<ProviderProfile>()
            .Include(p => p.User)
            .Where(p => p.IsVerified)
            .OrderByDescending(p => p.AverageRating)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProviderProfile>> GetFeaturedProvidersAsync()
    {
        return await _context.Set<ProviderProfile>()
            .Include(p => p.User)
            .Where(p => p.IsFeatured && p.IsVerified)
            .OrderByDescending(p => p.AverageRating)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProviderProfile>> SearchProvidersAsync(string? searchTerm, ProviderType? providerType, string? coverageArea)
    {
        var query = _context.Set<ProviderProfile>()
            .Include(p => p.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.BusinessName.Contains(searchTerm) || 
                (p.Description != null && p.Description.Contains(searchTerm)));
        }

        if (providerType.HasValue)
        {
            query = query.Where(p => p.ProviderType == providerType.Value);
        }

        if (!string.IsNullOrWhiteSpace(coverageArea))
        {
            query = query.Where(p => p.CoverageAreas.Contains(coverageArea));
        }

        return await query
            .OrderByDescending(p => p.IsVerified)
            .ThenByDescending(p => p.AverageRating)
            .ToListAsync();
    }

    public async Task UpdateAverageRatingAsync(Guid providerId, decimal newRating)
    {
        var provider = await GetByIdAsync(providerId);
        if (provider != null)
        {
            provider.TotalRatings++;
            provider.AverageRating = ((provider.AverageRating * (provider.TotalRatings - 1)) + newRating) / provider.TotalRatings;
            provider.UpdatedAt = DateTime.UtcNow;
            Update(provider);
            await SaveChangesAsync();
        }
    }

    public async Task UpdateCompletionRateAsync(Guid providerId)
    {
        var provider = await GetByIdAsync(providerId);
        if (provider != null && provider.TotalProjects > 0)
        {
            // This will be calculated based on actual project completion data
            // For now, this is a placeholder
            provider.UpdatedAt = DateTime.UtcNow;
            Update(provider);
            await SaveChangesAsync();
        }
    }
}
