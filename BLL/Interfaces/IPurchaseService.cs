using ChineseAuction.Models.DTOs;

namespace ChineseAuction.BLL.Interfaces;

public interface IPurchaseService
{
    Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync();
    Task<IEnumerable<PurchaseDto>> GetUserPurchasesAsync(int userId);
    Task<IEnumerable<PurchaseDto>> GetMostExpensivePurchasesAsync(int count = 10);
    Task<IEnumerable<PurchaseDto>> GetMostPurchasedGiftsAsync(int count = 10);
    Task<SalesReportDto> GetSalesReportAsync();
}