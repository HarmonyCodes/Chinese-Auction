using ChineseAuction.Models.Entities;

namespace ChineseAuction.DAL.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetUserCartAsync(int userId);
    Task<CartItem> AddToCartAsync(CartItem cartItem);
    Task<bool> RemoveFromCartAsync(int userId, int giftId);
    Task<bool> ClearCartAsync(int userId);
    Task<bool> FinalizeCartAsync(int userId);
    Task<bool> IsItemInCartAsync(int userId, int giftId);
}