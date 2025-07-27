using ChineseAuction.Models.DTOs;

namespace ChineseAuction.BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterCustomerAsync(RegisterRequest request);
    Task<string> GenerateJwtTokenAsync(int userId, string email, string role);
    Task<bool> ValidateTokenAsync(string token);
}