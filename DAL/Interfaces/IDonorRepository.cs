using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Interfaces;

public interface IDonorRepository
{
    Task<IEnumerable<Donor>> GetAllAsync();
    Task<Donor?> GetByIdAsync(int id);
    Task<Donor> CreateAsync(Donor donor);
    Task<Donor> UpdateAsync(Donor donor);
    Task<bool> DeleteAsync(int id);
    Task<Donor?> GetWithGiftsAsync(int id);
}