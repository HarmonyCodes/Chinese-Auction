using ChineseAuction.BLL.Interfaces;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.DTOs;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.BLL.Services;

public class DonorService : IDonorService
{
    private readonly IDonorRepository _donorRepository;
    private readonly ILogger<DonorService> _logger;

    public DonorService(IDonorRepository donorRepository, ILogger<DonorService> logger)
    {
        _donorRepository = donorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<DonorDto>> GetAllDonorsAsync()
    {
        try
        {
            var donors = await _donorRepository.GetAllAsync();
            return donors.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all donors");
            throw;
        }
    }

    public async Task<DonorDto?> GetDonorByIdAsync(int id)
    {
        try
        {
            var donor = await _donorRepository.GetWithGiftsAsync(id);
            return donor == null ? null : MapToDto(donor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving donor by ID: {Id}", id);
            throw;
        }
    }

    public async Task<DonorDto> CreateDonorAsync(CreateDonorRequest request)
    {
        try
        {
            var donor = new Donor
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone
            };

            donor = await _donorRepository.CreateAsync(donor);
            _logger.LogInformation("Donor created successfully: {Name}", donor.Name);
            return MapToDto(donor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating donor: {Name}", request.Name);
            throw;
        }
    }

    public async Task<DonorDto> UpdateDonorAsync(int id, CreateDonorRequest request)
    {
        try
        {
            var donor = await _donorRepository.GetByIdAsync(id);
            if (donor == null)
            {
                throw new KeyNotFoundException($"Donor with ID {id} not found");
            }

            donor.Name = request.Name;
            donor.Email = request.Email;
            donor.Phone = request.Phone;

            donor = await _donorRepository.UpdateAsync(donor);
            _logger.LogInformation("Donor updated successfully: {Id}", id);
            return MapToDto(donor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating donor: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDonorAsync(int id)
    {
        try
        {
            var result = await _donorRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Donor deleted successfully: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Donor not found for deletion: {Id}", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting donor: {Id}", id);
            throw;
        }
    }

    private static DonorDto MapToDto(Donor donor)
    {
        return new DonorDto
        {
            Id = donor.Id,
            Name = donor.Name,
            Email = donor.Email,
            Phone = donor.Phone,
            CreatedAt = donor.CreatedAt,
            Gifts = donor.Gifts.Select(g => new GiftSummaryDto
            {
                Id = g.Id,
                Name = g.Name,
                Category = g.Category,
                Price = g.Price,
                IsRaffled = g.IsRaffled,
                BuyerCount = g.Purchases.Count
            }).ToList()
        };
    }
}