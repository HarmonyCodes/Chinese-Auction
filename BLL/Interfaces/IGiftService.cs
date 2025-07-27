using ChineseAuction.Models.DTOs;

namespace ChineseAuction.BLL.Interfaces;

public interface IGiftService
{
    Task<IEnumerable<GiftDto>> GetAllGiftsAsync();
    Task<GiftDto?> GetGiftByIdAsync(int id);
    Task<GiftDto> CreateGiftAsync(CreateGiftRequest request);
    Task<GiftDto> UpdateGiftAsync(int id, CreateGiftRequest request);
    Task<bool> DeleteGiftAsync(int id);
    Task<IEnumerable<GiftDto>> SearchGiftsAsync(string? name = null, string? donorName = null, int? minBuyers = null);
    Task<IEnumerable<GiftDto>> GetGiftsByCategoryAsync(string category);
    Task<IEnumerable<string>> GetCategoriesAsync();
}