using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChineseAuction.BLL.Interfaces;
using ChineseAuction.Models.DTOs;

namespace ChineseAuction.Controllers;

/// <summary>
/// Raffle management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RaffleController : ControllerBase
{
    private readonly IRaffleService _raffleService;
    private readonly ILogger<RaffleController> _logger;

    public RaffleController(IRaffleService raffleService, ILogger<RaffleController> logger)
    {
        _raffleService = raffleService;
        _logger = logger;
    }

    /// <summary>
    /// Get gifts available for raffle (Manager only)
    /// </summary>
    /// <returns>List of gifts that can be raffled</returns>
    [HttpGet("available")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<IEnumerable<GiftDto>>> GetAvailableForRaffle()
    {
        try
        {
            _logger.LogInformation("Retrieving gifts available for raffle");
            var gifts = await _raffleService.GetGiftsAvailableForRaffleAsync();
            return Ok(gifts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gifts available for raffle");
            return StatusCode(500, new { message = "An error occurred while retrieving gifts available for raffle" });
        }
    }

    /// <summary>
    /// Run raffle for specific gift (Manager only)
    /// </summary>
    /// <param name="giftId">Gift ID to raffle</param>
    /// <returns>Raffle result with winner details</returns>
    [HttpPost("run/{giftId}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<RaffleResultDto>> RunRaffle(int giftId)
    {
        try
        {
            _logger.LogInformation("Running raffle for gift: {GiftId}", giftId);
            var result = await _raffleService.RunRaffleForGiftAsync(giftId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Gift not found for raffle: {GiftId}", giftId);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid raffle operation for gift: {GiftId}", giftId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running raffle for gift: {GiftId}", giftId);
            return StatusCode(500, new { message = "An error occurred while running the raffle" });
        }
    }

    /// <summary>
    /// Run raffle for all eligible gifts (Manager only)
    /// </summary>
    /// <returns>List of raffle results</returns>
    [HttpPost("run-all")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<IEnumerable<RaffleResultDto>>> RunAllRaffles()
    {
        try
        {
            _logger.LogInformation("Running raffle for all eligible gifts");
            var results = await _raffleService.RunRaffleForAllGiftsAsync();
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running raffle for all gifts");
            return StatusCode(500, new { message = "An error occurred while running the raffles" });
        }
    }

    /// <summary>
    /// Get raffle results (accessible to all authenticated users)
    /// </summary>
    /// <returns>List of completed raffle results</returns>
    [HttpGet("results")]
    public async Task<ActionResult<IEnumerable<RaffleResultDto>>> GetRaffleResults()
    {
        try
        {
            _logger.LogInformation("Retrieving raffle results");
            var results = await _raffleService.GetRaffleResultsAsync();
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving raffle results");
            return StatusCode(500, new { message = "An error occurred while retrieving raffle results" });
        }
    }
}