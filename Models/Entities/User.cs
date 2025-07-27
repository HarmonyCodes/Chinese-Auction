using System.ComponentModel.DataAnnotations;

namespace ChineseAuction.Models.Entities;

public class User
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required, MaxLength(20)]
    public string Role { get; set; } = "customer"; // "manager" or "customer"
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}