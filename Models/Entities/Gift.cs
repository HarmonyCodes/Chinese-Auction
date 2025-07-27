using System.ComponentModel.DataAnnotations;

namespace ChineseAuction.Models.Entities;

public class Gift
{
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    public decimal Price { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    public bool IsRaffled { get; set; } = false;
    public int? WinnerId { get; set; }
    public DateTime? RaffleDate { get; set; }
    
    // Foreign key
    public int DonorId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Donor Donor { get; set; } = null!;
    public virtual User? Winner { get; set; }
    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}