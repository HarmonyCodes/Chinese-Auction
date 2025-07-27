using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChineseAuction.BLL.Interfaces;
using ChineseAuction.Models.DTOs;

namespace ChineseAuction.Controllers;

/// <summary>
/// Gift management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GiftsController : ControllerBase
{
    private readonly IGiftService _giftService;
    private readonly ILogger<GiftsController> _logger;

    public GiftsController(IGiftService giftService, ILogger<GiftsController> logger)
    {
        _giftService = giftService;
        _logger = logger;
    }

    /// <summary>
    /// Get all gifts (accessible to customers and managers)
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="sortBy">Sort by: price, category</param>
    /// <returns>List of gifts</returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<GiftDto>>> GetAllGifts([FromQuery] string? category = null, [FromQuery] string? sortBy = null)
    {
        try
        {
            _logger.LogInformation("Retrieving gifts with category: {Category}, sortBy: {SortBy}", category, sortBy);
            
            IEnumerable<GiftDto> gifts;
            
            if (!string.IsNullOrEmpty(category))
            {
                gifts = await _giftService.GetGiftsByCategoryAsync(category);
            }
            else
            {
                gifts = await _giftService.GetAllGiftsAsync();
            }

            // Apply sorting
            gifts = sortBy?.ToLower() switch
            {
                "price" => gifts.OrderBy(g => g.Price),
                "category" => gifts.OrderBy(g => g.Category),
                _ => gifts.OrderBy(g => g.Name)
            };

            return Ok(gifts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gifts");
            return StatusCode(500, new { message = "An error occurred while retrieving gifts" });
        }
    }

    /// <summary>
    /// Search gifts by name, donor name, or number of buyers (Manager only)
    /// </summary>
    /// <param name="name">Gift name search</param>
    /// <param name="donorName">Donor name search</param>
    /// <param name="minBuyers">Minimum number of buyers</param>
    /// <returns>Filtered gifts</returns>
    [HttpGet("search")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<IEnumerable<GiftDto>>> SearchGifts([FromQuery] string? name = null, [FromQuery] string? donorName = null, [FromQuery] int? minBuyers = null)
    {
        try
        {
            _logger.LogInformation("Searching gifts with name: {Name}, donorName: {DonorName}, minBuyers: {MinBuyers}", name, donorName, minBuyers);
            var gifts = await _giftService.SearchGiftsAsync(name, donorName, minBuyers);
            return Ok(gifts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gifts");
            return StatusCode(500, new { message = "An error occurred while searching gifts" });
        }
    }

    /// <summary>
    /// Get gift categories
    /// </summary>
    /// <returns>List of categories</returns>
    [HttpGet("categories")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        try
        {
            _logger.LogInformation("Retrieving gift categories");
            var categories = await _giftService.GetCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return StatusCode(500, new { message = "An error occurred while retrieving categories" });
        }
    }

    /// <summary>
    /// Get gift by ID
    /// </summary>
    /// <param name="id">Gift ID</param>
    /// <returns>Gift details</returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GiftDto>> GetGift(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving gift: {Id}", id);
            var gift = await _giftService.GetGiftByIdAsync(id);
            
            if (gift == null)
            {
                return NotFound(new { message = $"Gift with ID {id} not found" });
            }

            return Ok(gift);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gift: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the gift" });
        }
    }

    /// <summary>
    /// Create a new gift (Manager only)
    /// </summary>
    /// <param name="request">Gift creation request</param>
    /// <returns>Created gift</returns>
    [HttpPost]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<GiftDto>> CreateGift([FromBody] CreateGiftRequest request)
    {
        try
        {
            _logger.LogInformation("Creating gift: {Name}", request.Name);
            var gift = await _giftService.CreateGiftAsync(request);
            return CreatedAtAction(nameof(GetGift), new { id = gift.Id }, gift);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Donor not found for gift creation: {DonorId}", request.DonorId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gift: {Name}", request.Name);
            return StatusCode(500, new { message = "An error occurred while creating the gift" });
        }
    }

    /// <summary>
    /// Update an existing gift (Manager only)
    /// </summary>
    /// <param name="id">Gift ID</param>
    /// <param name="request">Updated gift data</param>
    /// <returns>Updated gift</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<GiftDto>> UpdateGift(int id, [FromBody] CreateGiftRequest request)
    {
        try
        {
            _logger.LogInformation("Updating gift: {Id}", id);
            var gift = await _giftService.UpdateGiftAsync(id, request);
            return Ok(gift);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Gift or donor not found for update: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gift: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the gift" });
        }
    }

    /// <summary>
    /// Delete a gift (Manager only)
    /// </summary>
    /// <param name="id">Gift ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult> DeleteGift(int id)
    {
        try
        {
            _logger.LogInformation("Deleting gift: {Id}", id);
            var result = await _giftService.DeleteGiftAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = $"Gift with ID {id} not found" });
            }

            return Ok(new { message = "Gift deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gift: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the gift" });
        }
    }
}