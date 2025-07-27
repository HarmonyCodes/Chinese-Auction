using ChineseAuction.BLL.Interfaces;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.DTOs;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.BLL.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IGiftRepository _giftRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ILogger<CartService> _logger;

    public CartService(ICartRepository cartRepository, IGiftRepository giftRepository, 
        IPurchaseRepository purchaseRepository, ILogger<CartService> logger)
    {
        _cartRepository = cartRepository;
        _giftRepository = giftRepository;
        _purchaseRepository = purchaseRepository;
        _logger = logger;
    }

    public async Task<CartDto> GetUserCartAsync(int userId)
    {
        try
        {
            var cartItems = await _cartRepository.GetUserCartAsync(userId);
            return MapToCartDto(cartItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<CartDto> AddToCartAsync(int userId, AddToCartRequest request)
    {
        try
        {
            var gift = await _giftRepository.GetByIdAsync(request.GiftId);
            if (gift == null)
            {
                throw new KeyNotFoundException($"Gift with ID {request.GiftId} not found");
            }

            if (gift.IsRaffled)
            {
                throw new InvalidOperationException("Cannot purchase a raffled gift");
            }

            if (await _cartRepository.IsItemInCartAsync(userId, request.GiftId))
            {
                throw new InvalidOperationException("Gift is already in cart");
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                GiftId = request.GiftId,
                IsDraft = true
            };

            await _cartRepository.AddToCartAsync(cartItem);
            _logger.LogInformation("Gift added to cart: User {UserId}, Gift {GiftId}", userId, request.GiftId);

            var cartItems = await _cartRepository.GetUserCartAsync(userId);
            return MapToCartDto(cartItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding gift to cart: User {UserId}, Gift {GiftId}", userId, request.GiftId);
            throw;
        }
    }

    public async Task<bool> RemoveFromCartAsync(int userId, int giftId)
    {
        try
        {
            var result = await _cartRepository.RemoveFromCartAsync(userId, giftId);
            if (result)
            {
                _logger.LogInformation("Gift removed from cart: User {UserId}, Gift {GiftId}", userId, giftId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing gift from cart: User {UserId}, Gift {GiftId}", userId, giftId);
            throw;
        }
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        try
        {
            var result = await _cartRepository.ClearCartAsync(userId);
            if (result)
            {
                _logger.LogInformation("Cart cleared for user: {UserId}", userId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<CartDto> FinishOrderAsync(int userId)
    {
        try
        {
            var cartItems = await _cartRepository.GetUserCartAsync(userId);
            var draftItems = cartItems.Where(c => c.IsDraft).ToList();

            if (!draftItems.Any())
            {
                throw new InvalidOperationException("No items in cart to finalize");
            }

            // Create purchases from cart items
            var purchases = draftItems.Select(item => new Purchase
            {
                UserId = userId,
                GiftId = item.GiftId,
                PricePaid = item.Gift.Price,
                PurchaseDate = DateTime.UtcNow
            });

            await _purchaseRepository.CreateBulkAsync(purchases);

            // Mark cart items as finalized (not draft)
            await _cartRepository.FinalizeCartAsync(userId);

            _logger.LogInformation("Order finalized for user: {UserId}, Items: {ItemCount}", userId, draftItems.Count);

            var updatedCartItems = await _cartRepository.GetUserCartAsync(userId);
            return MapToCartDto(updatedCartItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finalizing order for user: {UserId}", userId);
            throw;
        }
    }

    private static CartDto MapToCartDto(IEnumerable<CartItem> cartItems)
    {
        var items = cartItems.Select(c => new CartItemDto
        {
            Id = c.Id,
            GiftId = c.GiftId,
            GiftName = c.Gift.Name,
            GiftCategory = c.Gift.Category,
            Price = c.Gift.Price,
            ImageUrl = c.Gift.ImageUrl,
            IsDraft = c.IsDraft,
            CreatedAt = c.CreatedAt
        }).ToList();

        var totalAmount = items.Where(i => i.IsDraft).Sum(i => i.Price);
        var isDraft = items.Any(i => i.IsDraft);

        return new CartDto
        {
            Items = items,
            TotalAmount = totalAmount,
            IsDraft = isDraft
        };
    }
}