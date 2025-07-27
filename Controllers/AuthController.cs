using Microsoft.AspNetCore.Mvc;
using ChineseAuction.BLL.Interfaces;
using ChineseAuction.Models.DTOs;

namespace ChineseAuction.Controllers;

/// <summary>
/// Authentication controller for user login and registration
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login for managers and customers
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized login attempt for email: {Email}", request.Email);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Register a new customer account
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);
            var response = await _authService.RegisterCustomerAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for email: {Email}", request.Email);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }
}