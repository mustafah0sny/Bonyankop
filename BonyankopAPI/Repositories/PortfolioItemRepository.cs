using BonyankopAPI.Data;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BonyankopAPI.Repositories;

public class PortfolioItemRepository : Repository<PortfolioItem>, IPortfolioItemRepository
{
    public PortfolioItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PortfolioItem>> GetByProviderIdAsync(Guid providerId)
    {
        return await _context.Set<PortfolioItem>()
            .Where(p => p.ProviderId == providerId)
            .OrderBy(p => p.DisplayOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PortfolioItem>> GetFeaturedByProviderIdAsync(Guid providerId)
    {
        return await _context.Set<PortfolioItem>()
            .Where(p => p.ProviderId == providerId && p.IsFeatured)
            .OrderBy(p => p.DisplayOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PortfolioItem?> GetByIdAndProviderIdAsync(Guid portfolioId, Guid providerId)
    {
        return await _context.Set<PortfolioItem>()
            .FirstOrDefaultAsync(p => p.PortfolioId == portfolioId && p.ProviderId == providerId);
    }

    public async Task<int> GetMaxDisplayOrderAsync(Guid providerId)
    {
        var maxOrder = await _context.Set<PortfolioItem>()
            .Where(p => p.ProviderId == providerId)
            .MaxAsync(p => (int?)p.DisplayOrder);
        
        return maxOrder ?? 0;
    }

    public async Task ReorderAsync(Guid providerId, Guid portfolioId, int newOrder)
    {
        var item = await GetByIdAndProviderIdAsync(portfolioId, providerId);
        if (item == null) return;

        var oldOrder = item.DisplayOrder;
        
        if (newOrder == oldOrder) return;

        var items = await _context.Set<PortfolioItem>()
            .Where(p => p.ProviderId == providerId)
            .ToListAsync();

        if (newOrder > oldOrder)
        {
            // Moving down
            foreach (var p in items.Where(p => p.DisplayOrder > oldOrder && p.DisplayOrder <= newOrder))
            {
                p.DisplayOrder--;
            }
        }
        else
        {
            // Moving up
            foreach (var p in items.Where(p => p.DisplayOrder >= newOrder && p.DisplayOrder < oldOrder))
            {
                p.DisplayOrder++;
            }
        }

        item.DisplayOrder = newOrder;
        item.UpdatedAt = DateTime.UtcNow;
        
        await SaveChangesAsync();
    }
}
