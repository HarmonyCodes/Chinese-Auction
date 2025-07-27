using Microsoft.EntityFrameworkCore;
using ChineseAuction.Data;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Repositories;

public class DonorRepository : IDonorRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DonorRepository> _logger;

    public DonorRepository(ApplicationDbContext context, ILogger<DonorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Donor>> GetAllAsync()
    {
        try
        {
            return await _context.Donors
                .Include(d => d.Gifts)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all donors");
            throw;
        }
    }

    public async Task<Donor?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Donors.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving donor by ID: {Id}", id);
            throw;
        }
    }

    public async Task<Donor> CreateAsync(Donor donor)
    {
        try
        {
            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Donor created successfully: {Name}", donor.Name);
            return donor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating donor: {Name}", donor.Name);
            throw;
        }
    }

    public async Task<Donor> UpdateAsync(Donor donor)
    {
        try
        {
            _context.Donors.Update(donor);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Donor updated successfully: {Id}", donor.Id);
            return donor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating donor: {Id}", donor.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null) return false;

            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Donor deleted successfully: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting donor: {Id}", id);
            throw;
        }
    }

    public async Task<Donor?> GetWithGiftsAsync(int id)
    {
        try
        {
            return await _context.Donors
                .Include(d => d.Gifts)
                .ThenInclude(g => g.Purchases)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving donor with gifts: {Id}", id);
            throw;
        }
    }
}