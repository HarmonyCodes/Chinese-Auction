using ChineseAuction.Models.DTOs;

namespace ChineseAuction.BLL.Interfaces;

public interface ICartService
{
    Task<CartDto> GetUserCartAsync(int userId);
    Task<CartDto> AddToCartAsync(int userId, AddToCartRequest request);
    Task<bool> RemoveFromCartAsync(int userId, int giftId);
    Task<bool> ClearCartAsync(int userId);
    Task<CartDto> FinishOrderAsync(int userId);
}