using ChineseAuction.BLL.Interfaces;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.DTOs;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.BLL.Services;

public class GiftService : IGiftService
{
    private readonly IGiftRepository _giftRepository;
    private readonly IDonorRepository _donorRepository;
    private readonly ILogger<GiftService> _logger;

    public GiftService(IGiftRepository giftRepository, IDonorRepository donorRepository, ILogger<GiftService> logger)
    {
        _giftRepository = giftRepository;
        _donorRepository = donorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<GiftDto>> GetAllGiftsAsync()
    {
        try
        {
            var gifts = await _giftRepository.GetAllAsync();
            return gifts.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all gifts");
            throw;
        }
    }

    public async Task<GiftDto?> GetGiftByIdAsync(int id)
    {
        try
        {
            var gift = await _giftRepository.GetWithDetailsAsync(id);
            return gift == null ? null : MapToDto(gift);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gift by ID: {Id}", id);
            throw;
        }
    }

    public async Task<GiftDto> CreateGiftAsync(CreateGiftRequest request)
    {
        try
        {
            var donor = await _donorRepository.GetByIdAsync(request.DonorId);
            if (donor == null)
            {
                throw new KeyNotFoundException($"Donor with ID {request.DonorId} not found");
            }

            var gift = new Gift
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                DonorId = request.DonorId
            };

            gift = await _giftRepository.CreateAsync(gift);
            _logger.LogInformation("Gift created successfully: {Name}", gift.Name);
            
            // Reload with relationships
            gift = await _giftRepository.GetWithDetailsAsync(gift.Id);
            return MapToDto(gift!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gift: {Name}", request.Name);
            throw;
        }
    }

    public async Task<GiftDto> UpdateGiftAsync(int id, CreateGiftRequest request)
    {
        try
        {
            var gift = await _giftRepository.GetByIdAsync(id);
            if (gift == null)
            {
                throw new KeyNotFoundException($"Gift with ID {id} not found");
            }

            var donor = await _donorRepository.GetByIdAsync(request.DonorId);
            if (donor == null)
            {
                throw new KeyNotFoundException($"Donor with ID {request.DonorId} not found");
            }

            gift.Name = request.Name;
            gift.Description = request.Description;
            gift.Category = request.Category;
            gift.Price = request.Price;
            gift.ImageUrl = request.ImageUrl;
            gift.DonorId = request.DonorId;

            gift = await _giftRepository.UpdateAsync(gift);
            _logger.LogInformation("Gift updated successfully: {Id}", id);
            
            // Reload with relationships
            gift = await _giftRepository.GetWithDetailsAsync(gift.Id);
            return MapToDto(gift!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gift: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteGiftAsync(int id)
    {
        try
        {
            var result = await _giftRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Gift deleted successfully: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Gift not found for deletion: {Id}", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gift: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<GiftDto>> SearchGiftsAsync(string? name = null, string? donorName = null, int? minBuyers = null)
    {
        try
        {
            var gifts = await _giftRepository.SearchAsync(name, donorName, minBuyers);
            return gifts.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gifts");
            throw;
        }
    }

    public async Task<IEnumerable<GiftDto>> GetGiftsByCategoryAsync(string category)
    {
        try
        {
            var gifts = await _giftRepository.GetByCategoryAsync(category);
            return gifts.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gifts by category: {Category}", category);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        try
        {
            var gifts = await _giftRepository.GetAllAsync();
            return gifts.Select(g => g.Category).Distinct().OrderBy(c => c);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            throw;
        }
    }

    private static GiftDto MapToDto(Gift gift)
    {
        return new GiftDto
        {
            Id = gift.Id,
            Name = gift.Name,
            Description = gift.Description,
            Category = gift.Category,
            Price = gift.Price,
            ImageUrl = gift.ImageUrl,
            IsRaffled = gift.IsRaffled,
            WinnerName = gift.Winner?.Name,
            RaffleDate = gift.RaffleDate,
            DonorId = gift.DonorId,
            DonorName = gift.Donor?.Name ?? string.Empty,
            BuyerCount = gift.Purchases?.Count ?? 0,
            CreatedAt = gift.CreatedAt
        };
    }
}