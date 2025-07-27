using ChineseAuction.BLL.Interfaces;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.DTOs;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.BLL.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ILogger<PurchaseService> _logger;

    public PurchaseService(IPurchaseRepository purchaseRepository, ILogger<PurchaseService> logger)
    {
        _purchaseRepository = purchaseRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync()
    {
        try
        {
            var purchases = await _purchaseRepository.GetAllAsync();
            return purchases.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all purchases");
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseDto>> GetUserPurchasesAsync(int userId)
    {
        try
        {
            var purchases = await _purchaseRepository.GetByUserIdAsync(userId);
            return purchases.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving purchases for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseDto>> GetMostExpensivePurchasesAsync(int count = 10)
    {
        try
        {
            var purchases = await _purchaseRepository.GetMostExpensiveAsync(count);
            return purchases.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving most expensive purchases");
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseDto>> GetMostPurchasedGiftsAsync(int count = 10)
    {
        try
        {
            var purchases = await _purchaseRepository.GetMostPurchasedGiftsAsync(count);
            return purchases.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving most purchased gifts");
            throw;
        }
    }

    public async Task<SalesReportDto> GetSalesReportAsync()
    {
        try
        {
            var totalIncome = await _purchaseRepository.GetTotalIncomeAsync();
            var allPurchases = await _purchaseRepository.GetAllAsync();
            var categoryData = await _purchaseRepository.GetSalesByCategoryAsync();

            var totalPurchases = allPurchases.Count();
            var totalCustomers = allPurchases.Select(p => p.UserId).Distinct().Count();

            var categoryBreakdown = categoryData.Select(c => new CategorySalesDto
            {
                Category = c.GetType().GetProperty("Category")?.GetValue(c)?.ToString() ?? "",
                TotalSales = (decimal)(c.GetType().GetProperty("TotalSales")?.GetValue(c) ?? 0),
                ItemCount = (int)(c.GetType().GetProperty("ItemCount")?.GetValue(c) ?? 0)
            }).ToList();

            var topGifts = allPurchases
                .GroupBy(p => new { p.GiftId, p.Gift.Name, p.Gift.Price })
                .Select(g => new TopGiftDto
                {
                    GiftId = g.Key.GiftId,
                    GiftName = g.Key.Name,
                    Price = g.Key.Price,
                    PurchaseCount = g.Count(),
                    TotalRevenue = g.Sum(p => p.PricePaid)
                })
                .OrderByDescending(g => g.PurchaseCount)
                .Take(10)
                .ToList();

            return new SalesReportDto
            {
                TotalIncome = totalIncome,
                TotalPurchases = totalPurchases,
                TotalCustomers = totalCustomers,
                CategoryBreakdown = categoryBreakdown,
                TopGifts = topGifts
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales report");
            throw;
        }
    }

    private static PurchaseDto MapToDto(Purchase purchase)
    {
        return new PurchaseDto
        {
            Id = purchase.Id,
            UserId = purchase.UserId,
            UserName = purchase.User.Name,
            UserEmail = purchase.User.Email,
            UserPhone = purchase.User.Phone,
            GiftId = purchase.GiftId,
            GiftName = purchase.Gift.Name,
            GiftCategory = purchase.Gift.Category,
            DonorName = purchase.Gift.Donor?.Name ?? string.Empty,
            PricePaid = purchase.PricePaid,
            PurchaseDate = purchase.PurchaseDate
        };
    }
}