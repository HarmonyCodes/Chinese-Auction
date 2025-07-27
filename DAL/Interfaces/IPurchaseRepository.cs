using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Interfaces;

public interface IPurchaseRepository
{
    Task<IEnumerable<Purchase>> GetAllAsync();
    Task<IEnumerable<Purchase>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Purchase>> GetByGiftIdAsync(int giftId);
    Task<Purchase> CreateAsync(Purchase purchase);
    Task<IEnumerable<Purchase>> CreateBulkAsync(IEnumerable<Purchase> purchases);
    Task<IEnumerable<Purchase>> GetMostExpensiveAsync(int count = 10);
    Task<IEnumerable<Purchase>> GetMostPurchasedGiftsAsync(int count = 10);
    Task<decimal> GetTotalIncomeAsync();
    Task<IEnumerable<object>> GetSalesByCategoryAsync();
}