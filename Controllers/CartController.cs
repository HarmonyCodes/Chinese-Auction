using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChineseAuction.BLL.Interfaces;
using ChineseAuction.Models.DTOs;

namespace ChineseAuction.Controllers;

/// <summary>
/// Shopping cart controller for customers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "customer")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's cart
    /// </summary>
    /// <returns>User's shopping cart</returns>
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        try
        {
            var userId = (int)HttpContext.Items["UserId"]!;
            _logger.LogInformation("Retrieving cart for user: {UserId}", userId);
            var cart = await _cartService.GetUserCartAsync(userId);
            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user cart");
            return StatusCode(500, new { message = "An error occurred while retrieving your cart" });
        }
    }

    /// <summary>
    /// Add gift to cart
    /// </summary>
    /// <param name="request">Gift to add</param>
    /// <returns>Updated cart</returns>
    [HttpPost("add")]
    public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            var userId = (int)HttpContext.Items["UserId"]!;
            _logger.LogInformation("Adding gift to cart: User {UserId}, Gift {GiftId}", userId, request.GiftId);
            var cart = await _cartService.AddToCartAsync(userId, request);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Gift not found for cart addition");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid cart operation");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding gift to cart");
            return StatusCode(500, new { message = "An error occurred while adding the gift to your cart" });
        }
    }

    /// <summary>
    /// Remove gift from cart (only for draft items)
    /// </summary>
    /// <param name="giftId">Gift ID to remove</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("remove/{giftId}")]
    public async Task<ActionResult> RemoveFromCart(int giftId)
    {
        try
        {
            var userId = (int)HttpContext.Items["UserId"]!;
            _logger.LogInformation("Removing gift from cart: User {UserId}, Gift {GiftId}", userId, giftId);
            var result = await _cartService.RemoveFromCartAsync(userId, giftId);
            
            if (!result)
            {
                return BadRequest(new { message = "Cannot remove item - cart may be finalized or item not found" });
            }

            return Ok(new { message = "Item removed from cart successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing gift from cart");
            return StatusCode(500, new { message = "An error occurred while removing the item from your cart" });
        }
    }

    /// <summary>
    /// Clear all draft items from cart
    /// </summary>
    /// <returns>Success confirmation</returns>
    [HttpDelete("clear")]
    public async Task<ActionResult> ClearCart()
    {
        try
        {
            var userId = (int)HttpContext.Items["UserId"]!;
            _logger.LogInformation("Clearing cart for user: {UserId}", userId);
            await _cartService.ClearCartAsync(userId);
            return Ok(new { message = "Cart cleared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return StatusCode(500, new { message = "An error occurred while clearing your cart" });
        }
    }

    /// <summary>
    /// Finalize order - convert cart items to purchases
    /// </summary>
    /// <returns>Final cart status</returns>
    [HttpPost("finish-order")]
    public async Task<ActionResult<CartDto>> FinishOrder()
    {
        try
        {
            var userId = (int)HttpContext.Items["UserId"]!;
            _logger.LogInformation("Finalizing order for user: {UserId}", userId);
            var cart = await _cartService.FinishOrderAsync(userId);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid order finalization");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finalizing order");
            return StatusCode(500, new { message = "An error occurred while finalizing your order" });
        }
    }
}