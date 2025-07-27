using Microsoft.EntityFrameworkCore;
using ChineseAuction.Data;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CartRepository> _logger;

    public CartRepository(ApplicationDbContext context, ILogger<CartRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<CartItem>> GetUserCartAsync(int userId)
    {
        try
        {
            return await _context.CartItems
                .Include(c => c.Gift)
                .ThenInclude(g => g.Donor)
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<CartItem> AddToCartAsync(CartItem cartItem)
    {
        try
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Item added to cart: User {UserId}, Gift {GiftId}", 
                cartItem.UserId, cartItem.GiftId);
            return cartItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart: User {UserId}, Gift {GiftId}", 
                cartItem.UserId, cartItem.GiftId);
            throw;
        }
    }

    public async Task<bool> RemoveFromCartAsync(int userId, int giftId)
    {
        try
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.GiftId == giftId && c.IsDraft);
            
            if (cartItem == null) return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Item removed from cart: User {UserId}, Gift {GiftId}", userId, giftId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart: User {UserId}, Gift {GiftId}", userId, giftId);
            throw;
        }
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        try
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId && c.IsDraft)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Cart cleared for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> FinalizeCartAsync(int userId)
    {
        try
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId && c.IsDraft)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                item.IsDraft = false;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Cart finalized for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finalizing cart for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> IsItemInCartAsync(int userId, int giftId)
    {
        try
        {
            return await _context.CartItems
                .AnyAsync(c => c.UserId == userId && c.GiftId == giftId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if item is in cart: User {UserId}, Gift {GiftId}", userId, giftId);
            throw;
        }
    }
}