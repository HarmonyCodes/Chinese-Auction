using ChineseAuction.Models.DTOs;

namespace ChineseAuction.BLL.Interfaces;

public interface IRaffleService
{
    Task<IEnumerable<GiftDto>> GetGiftsAvailableForRaffleAsync();
    Task<RaffleResultDto> RunRaffleForGiftAsync(int giftId);
    Task<IEnumerable<RaffleResultDto>> RunRaffleForAllGiftsAsync();
    Task<IEnumerable<RaffleResultDto>> GetRaffleResultsAsync();
}