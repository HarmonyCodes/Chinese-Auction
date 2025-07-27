using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChineseAuction.BLL.Interfaces;
using ChineseAuction.Models.DTOs;

namespace ChineseAuction.Controllers;

/// <summary>
/// Purchase management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly ILogger<PurchasesController> _logger;

    public PurchasesController(IPurchaseService purchaseService, ILogger<PurchasesController> logger)
    {
        _purchaseService = purchaseService;
        _logger = logger;
    }

    /// <summary>
    /// Get all purchases (Manager only)
    /// </summary>
    /// <returns>List of all purchases</returns>
    [HttpGet]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetAllPurchases()
    {
        try
        {
            _logger.LogInformation("Retrieving all purchases");
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return Ok(purchases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all purchases");
            return StatusCode(500, new { message = "An error occurred while retrieving purchases" });
        }
    }

    /// <summary>
    /// Get user's own purchases
    /// </summary>
    /// <returns>List of user's purchases</returns>
    [HttpGet("my-purchases")]
    public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetMyPurchases()
    {
        try
        {
            var userId = (int)HttpContext.Items["UserId"]!;
            _logger.LogInformation("Retrieving purchases for user: {UserId}", userId);
            var purchases = await _purchaseService.GetUserPurchasesAsync(userId);
            return Ok(purchases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user purchases");
            return StatusCode(500, new { message = "An error occurred while retrieving your purchases" });
        }
    }

    /// <summary>
    /// Get most expensive purchases (Manager only)
    /// </summary>
    /// <param name="count">Number of results to return</param>
    /// <returns>List of most expensive purchases</returns>
    [HttpGet("most-expensive")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetMostExpensive([FromQuery] int count = 10)
    {
        try
        {
            _logger.LogInformation("Retrieving most expensive purchases: {Count}", count);
            var purchases = await _purchaseService.GetMostExpensivePurchasesAsync(count);
            return Ok(purchases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving most expensive purchases");
            return StatusCode(500, new { message = "An error occurred while retrieving most expensive purchases" });
        }
    }

    /// <summary>
    /// Get most purchased gifts (Manager only)
    /// </summary>
    /// <param name="count">Number of results to return</param>
    /// <returns>List of most purchased gifts</returns>
    [HttpGet("most-purchased")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetMostPurchased([FromQuery] int count = 10)
    {
        try
        {
            _logger.LogInformation("Retrieving most purchased gifts: {Count}", count);
            var purchases = await _purchaseService.GetMostPurchasedGiftsAsync(count);
            return Ok(purchases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving most purchased gifts");
            return StatusCode(500, new { message = "An error occurred while retrieving most purchased gifts" });
        }
    }

    /// <summary>
    /// Get sales report (Manager only)
    /// </summary>
    /// <returns>Comprehensive sales report</returns>
    [HttpGet("sales-report")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<SalesReportDto>> GetSalesReport()
    {
        try
        {
            _logger.LogInformation("Generating sales report");
            var report = await _purchaseService.GetSalesReportAsync();
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales report");
            return StatusCode(500, new { message = "An error occurred while generating the sales report" });
        }
    }
}