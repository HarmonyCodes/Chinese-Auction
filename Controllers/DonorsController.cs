using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChineseAuction.BLL.Interfaces;
using ChineseAuction.Models.DTOs;

namespace ChineseAuction.Controllers;

/// <summary>
/// Donor management controller - Manager only
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "manager")]
public class DonorsController : ControllerBase
{
    private readonly IDonorService _donorService;
    private readonly ILogger<DonorsController> _logger;

    public DonorsController(IDonorService donorService, ILogger<DonorsController> logger)
    {
        _donorService = donorService;
        _logger = logger;
    }

    /// <summary>
    /// Get all donors with their donation history
    /// </summary>
    /// <returns>List of donors</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonorDto>>> GetAllDonors()
    {
        try
        {
            _logger.LogInformation("Retrieving all donors");
            var donors = await _donorService.GetAllDonorsAsync();
            return Ok(donors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all donors");
            return StatusCode(500, new { message = "An error occurred while retrieving donors" });
        }
    }

    /// <summary>
    /// Get donor by ID with their gifts
    /// </summary>
    /// <param name="id">Donor ID</param>
    /// <returns>Donor details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<DonorDto>> GetDonor(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving donor: {Id}", id);
            var donor = await _donorService.GetDonorByIdAsync(id);
            
            if (donor == null)
            {
                return NotFound(new { message = $"Donor with ID {id} not found" });
            }

            return Ok(donor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving donor: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the donor" });
        }
    }

    /// <summary>
    /// Create a new donor
    /// </summary>
    /// <param name="request">Donor creation request</param>
    /// <returns>Created donor</returns>
    [HttpPost]
    public async Task<ActionResult<DonorDto>> CreateDonor([FromBody] CreateDonorRequest request)
    {
        try
        {
            _logger.LogInformation("Creating donor: {Name}", request.Name);
            var donor = await _donorService.CreateDonorAsync(request);
            return CreatedAtAction(nameof(GetDonor), new { id = donor.Id }, donor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating donor: {Name}", request.Name);
            return StatusCode(500, new { message = "An error occurred while creating the donor" });
        }
    }

    /// <summary>
    /// Update an existing donor
    /// </summary>
    /// <param name="id">Donor ID</param>
    /// <param name="request">Updated donor data</param>
    /// <returns>Updated donor</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<DonorDto>> UpdateDonor(int id, [FromBody] CreateDonorRequest request)
    {
        try
        {
            _logger.LogInformation("Updating donor: {Id}", id);
            var donor = await _donorService.UpdateDonorAsync(id, request);
            return Ok(donor);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Donor not found for update: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating donor: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the donor" });
        }
    }

    /// <summary>
    /// Delete a donor
    /// </summary>
    /// <param name="id">Donor ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDonor(int id)
    {
        try
        {
            _logger.LogInformation("Deleting donor: {Id}", id);
            var result = await _donorService.DeleteDonorAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = $"Donor with ID {id} not found" });
            }

            return Ok(new { message = "Donor deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting donor: {Id}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the donor" });
        }
    }
}