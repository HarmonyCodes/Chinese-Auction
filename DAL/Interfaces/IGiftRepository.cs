using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Interfaces;

public interface IGiftRepository
{
    Task<IEnumerable<Gift>> GetAllAsync();
    Task<Gift?> GetByIdAsync(int id);
    Task<Gift> CreateAsync(Gift gift);
    Task<Gift> UpdateAsync(Gift gift);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Gift>> SearchAsync(string? name = null, string? donorName = null, int? minBuyers = null);
    Task<IEnumerable<Gift>> GetByCategoryAsync(string category);
    Task<IEnumerable<Gift>> GetAvailableForRaffleAsync();
    Task<Gift?> GetWithDetailsAsync(int id);
}