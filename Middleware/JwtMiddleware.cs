using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ChineseAuction.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            await AttachUserToContextAsync(context, token);
        }

        await _next(context);
    }

    private async Task AttachUserToContextAsync(HttpContext context, string token)
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

            var principal = await Task.Run(() => tokenHandler.ValidateToken(token, validationParameters, out _));
            
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = principal.FindFirst(ClaimTypes.Email);
            var roleClaim = principal.FindFirst(ClaimTypes.Role);

            if (userIdClaim != null && emailClaim != null && roleClaim != null)
            {
                context.Items["UserId"] = int.Parse(userIdClaim.Value);
                context.Items["UserEmail"] = emailClaim.Value;
                context.Items["UserRole"] = roleClaim.Value;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            // Token validation failed, but don't throw - let the controller handle authorization
        }
    }
}