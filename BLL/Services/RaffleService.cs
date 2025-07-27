using ChineseAuction.BLL.Interfaces;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.DTOs;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.BLL.Services;

public class RaffleService : IRaffleService
{
    private readonly IGiftRepository _giftRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ILogger<RaffleService> _logger;
    private readonly Random _random;

    public RaffleService(IGiftRepository giftRepository, IPurchaseRepository purchaseRepository, ILogger<RaffleService> logger)
    {
        _giftRepository = giftRepository;
        _purchaseRepository = purchaseRepository;
        _logger = logger;
        _random = new Random();
    }

    public async Task<IEnumerable<GiftDto>> GetGiftsAvailableForRaffleAsync()
    {
        try
        {
            var gifts = await _giftRepository.GetAvailableForRaffleAsync();
            return gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Category = g.Category,
                Price = g.Price,
                ImageUrl = g.ImageUrl,
                IsRaffled = g.IsRaffled,
                DonorId = g.DonorId,
                DonorName = g.Donor?.Name ?? string.Empty,
                BuyerCount = g.Purchases?.Count ?? 0,
                CreatedAt = g.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gifts available for raffle");
            throw;
        }
    }

    public async Task<RaffleResultDto> RunRaffleForGiftAsync(int giftId)
    {
        try
        {
            var gift = await _giftRepository.GetWithDetailsAsync(giftId);
            if (gift == null)
            {
                throw new KeyNotFoundException($"Gift with ID {giftId} not found");
            }

            if (gift.IsRaffled)
            {
                throw new InvalidOperationException("Gift has already been raffled");
            }

            var purchases = await _purchaseRepository.GetByGiftIdAsync(giftId);
            var purchaseList = purchases.ToList();

            if (!purchaseList.Any())
            {
                throw new InvalidOperationException("No purchases found for this gift");
            }

            // Select random winner
            var randomIndex = _random.Next(purchaseList.Count);
            var winningPurchase = purchaseList[randomIndex];

            // Update gift with winner
            gift.IsRaffled = true;
            gift.WinnerId = winningPurchase.UserId;
            gift.RaffleDate = DateTime.UtcNow;

            await _giftRepository.UpdateAsync(gift);

            _logger.LogInformation("Raffle completed for gift: {GiftId}, Winner: {WinnerId}", giftId, winningPurchase.UserId);

            return new RaffleResultDto
            {
                GiftId = gift.Id,
                GiftName = gift.Name,
                Category = gift.Category,
                Price = gift.Price,
                DonorName = gift.Donor?.Name ?? string.Empty,
                WinnerId = winningPurchase.UserId,
                WinnerName = winningPurchase.User.Name,
                WinnerEmail = winningPurchase.User.Email,
                WinnerPhone = winningPurchase.User.Phone,
                RaffleDate = gift.RaffleDate.Value,
                TotalParticipants = purchaseList.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running raffle for gift: {GiftId}", giftId);
            throw;
        }
    }

    public async Task<IEnumerable<RaffleResultDto>> RunRaffleForAllGiftsAsync()
    {
        try
        {
            var availableGifts = await _giftRepository.GetAvailableForRaffleAsync();
            var results = new List<RaffleResultDto>();

            foreach (var gift in availableGifts)
            {
                try
                {
                    var result = await RunRaffleForGiftAsync(gift.Id);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to run raffle for gift: {GiftId}", gift.Id);
                }
            }

            _logger.LogInformation("Bulk raffle completed: {Count} gifts processed", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running raffle for all gifts");
            throw;
        }
    }

    public async Task<IEnumerable<RaffleResultDto>> GetRaffleResultsAsync()
    {
        try
        {
            var gifts = await _giftRepository.GetAllAsync();
            var raffledGifts = gifts.Where(g => g.IsRaffled).ToList();

            var results = new List<RaffleResultDto>();

            foreach (var gift in raffledGifts)
            {
                var purchases = await _purchaseRepository.GetByGiftIdAsync(gift.Id);
                var purchaseCount = purchases.Count();

                results.Add(new RaffleResultDto
                {
                    GiftId = gift.Id,
                    GiftName = gift.Name,
                    Category = gift.Category,
                    Price = gift.Price,
                    DonorName = gift.Donor?.Name ?? string.Empty,
                    WinnerId = gift.WinnerId,
                    WinnerName = gift.Winner?.Name,
                    WinnerEmail = gift.Winner?.Email,
                    WinnerPhone = gift.Winner?.Phone,
                    RaffleDate = gift.RaffleDate ?? DateTime.MinValue,
                    TotalParticipants = purchaseCount
                });
            }

            return results.OrderByDescending(r => r.RaffleDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving raffle results");
            throw;
        }
    }
}