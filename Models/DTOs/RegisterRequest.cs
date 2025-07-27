using System.ComponentModel.DataAnnotations;

namespace ChineseAuction.Models.DTOs;

public class RegisterRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required, Phone, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}