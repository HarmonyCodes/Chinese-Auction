using Microsoft.EntityFrameworkCore;
using ChineseAuction.Data;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
            throw;
        }
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Users.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {Id}", id);
            throw;
        }
    }

    public async Task<User> CreateAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User created successfully: {Email}", user.Email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", user.Email);
            throw;
        }
    }

    public async Task<User> UpdateAsync(User user)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User updated successfully: {Id}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {Id}", user.Id);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        try
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email exists: {Email}", email);
            throw;
        }
    }
}