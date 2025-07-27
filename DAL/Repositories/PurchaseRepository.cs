using Microsoft.EntityFrameworkCore;
using ChineseAuction.Data;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PurchaseRepository> _logger;

    public PurchaseRepository(ApplicationDbContext context, ILogger<PurchaseRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Purchase>> GetAllAsync()
    {
        try
        {
            return await _context.Purchases
                .Include(p => p.User)
                .Include(p => p.Gift)
                .ThenInclude(g => g.Donor)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all purchases");
            throw;
        }
    }

    public async Task<IEnumerable<Purchase>> GetByUserIdAsync(int userId)
    {
        try
        {
            return await _context.Purchases
                .Include(p => p.Gift)
                .ThenInclude(g => g.Donor)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchases for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Purchase>> GetByGiftIdAsync(int giftId)
    {
        try
        {
            return await _context.Purchases
                .Include(p => p.User)
                .Where(p => p.GiftId == giftId)
                .OrderBy(p => p.PurchaseDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchases for gift: {GiftId}", giftId);
            throw;
        }
    }

    public async Task<Purchase> CreateAsync(Purchase purchase)
    {
        try
        {
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Purchase created successfully: User {UserId}, Gift {GiftId}", 
                purchase.UserId, purchase.GiftId);
            return purchase;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase: User {UserId}, Gift {GiftId}", 
                purchase.UserId, purchase.GiftId);
            throw;
        }
    }

    public async Task<IEnumerable<Purchase>> CreateBulkAsync(IEnumerable<Purchase> purchases)
    {
        try
        {
            var purchaseList = purchases.ToList();
            _context.Purchases.AddRange(purchaseList);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Bulk purchases created successfully: {Count} items", purchaseList.Count);
            return purchaseList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bulk purchases");
            throw;
        }
    }

    public async Task<IEnumerable<Purchase>> GetMostExpensiveAsync(int count = 10)
    {
        try
        {
            return await _context.Purchases
                .Include(p => p.User)
                .Include(p => p.Gift)
                .ThenInclude(g => g.Donor)
                .OrderByDescending(p => p.PricePaid)
                .Take(count)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving most expensive purchases");
            throw;
        }
    }

    public async Task<IEnumerable<Purchase>> GetMostPurchasedGiftsAsync(int count = 10)
    {
        try
        {
            return await _context.Purchases
                .Include(p => p.Gift)
                .ThenInclude(g => g.Donor)
                .GroupBy(p => p.GiftId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .SelectMany(g => g.Take(1))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving most purchased gifts");
            throw;
        }
    }

    public async Task<decimal> GetTotalIncomeAsync()
    {
        try
        {
            return await _context.Purchases.SumAsync(p => p.PricePaid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total income");
            throw;
        }
    }

    public async Task<IEnumerable<object>> GetSalesByCategoryAsync()
    {
        try
        {
            return await _context.Purchases
                .Include(p => p.Gift)
                .GroupBy(p => p.Gift.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalSales = g.Sum(p => p.PricePaid),
                    ItemCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales by category");
            throw;
        }
    }
}