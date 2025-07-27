using ChineseAuction.Models.DTOs;

namespace ChineseAuction.BLL.Interfaces;

public interface IDonorService
{
    Task<IEnumerable<DonorDto>> GetAllDonorsAsync();
    Task<DonorDto?> GetDonorByIdAsync(int id);
    Task<DonorDto> CreateDonorAsync(CreateDonorRequest request);
    Task<DonorDto> UpdateDonorAsync(int id, CreateDonorRequest request);
    Task<bool> DeleteDonorAsync(int id);
}