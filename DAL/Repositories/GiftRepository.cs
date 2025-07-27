using Microsoft.EntityFrameworkCore;
using ChineseAuction.Data;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Repositories;

public class GiftRepository : IGiftRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GiftRepository> _logger;

    public GiftRepository(ApplicationDbContext context, ILogger<GiftRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Gift>> GetAllAsync()
    {
        try
        {
            return await _context.Gifts
                .Include(g => g.Donor)
                .Include(g => g.Winner)
                .Include(g => g.Purchases)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all gifts");
            throw;
        }
    }

    public async Task<Gift?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Gifts.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gift by ID: {Id}", id);
            throw;
        }
    }

    public async Task<Gift> CreateAsync(Gift gift)
    {
        try
        {
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Gift created successfully: {Name}", gift.Name);
            return gift;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gift: {Name}", gift.Name);
            throw;
        }
    }

    public async Task<Gift> UpdateAsync(Gift gift)
    {
        try
        {
            _context.Gifts.Update(gift);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Gift updated successfully: {Id}", gift.Id);
            return gift;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gift: {Id}", gift.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null) return false;

            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Gift deleted successfully: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gift: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Gift>> SearchAsync(string? name = null, string? donorName = null, int? minBuyers = null)
    {
        try
        {
            var query = _context.Gifts
                .Include(g => g.Donor)
                .Include(g => g.Winner)
                .Include(g => g.Purchases)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(g => g.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(donorName))
            {
                query = query.Where(g => g.Donor.Name.Contains(donorName));
            }

            if (minBuyers.HasValue)
            {
                query = query.Where(g => g.Purchases.Count >= minBuyers.Value);
            }

            return await query.OrderBy(g => g.Name).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gifts");
            throw;
        }
    }

    public async Task<IEnumerable<Gift>> GetByCategoryAsync(string category)
    {
        try
        {
            return await _context.Gifts
                .Include(g => g.Donor)
                .Include(g => g.Winner)
                .Include(g => g.Purchases)
                .Where(g => g.Category == category)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gifts by category: {Category}", category);
            throw;
        }
    }

    public async Task<IEnumerable<Gift>> GetAvailableForRaffleAsync()
    {
        try
        {
            return await _context.Gifts
                .Include(g => g.Donor)
                .Include(g => g.Purchases)
                .Where(g => !g.IsRaffled && g.Purchases.Any())
                .OrderBy(g => g.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gifts available for raffle");
            throw;
        }
    }

    public async Task<Gift?> GetWithDetailsAsync(int id)
    {
        try
        {
            return await _context.Gifts
                .Include(g => g.Donor)
                .Include(g => g.Winner)
                .Include(g => g.Purchases)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gift with details: {Id}", id);
            throw;
        }
    }
}