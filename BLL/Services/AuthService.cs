using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ChineseAuction.BLL.Interfaces;
using ChineseAuction.DAL.Interfaces;
using ChineseAuction.Models.DTOs;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = await GenerateJwtTokenAsync(user.Id, user.Email, user.Role);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            _logger.LogInformation("User logged in successfully: {Email}", user.Email);

            return new AuthResponse
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponse> RegisterCustomerAsync(RegisterRequest request)
    {
        try
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "customer"
            };

            user = await _userRepository.CreateAsync(user);
            var token = await GenerateJwtTokenAsync(user.Id, user.Email, user.Role);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            _logger.LogInformation("Customer registered successfully: {Email}", user.Email);

            return new AuthResponse
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during customer registration for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<string> GenerateJwtTokenAsync(int userId, string email, string role)
    {
        try
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return await Task.FromResult(tokenHandler.WriteToken(token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            await Task.Run(() => tokenHandler.ValidateToken(token, validationParameters, out _));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }
}