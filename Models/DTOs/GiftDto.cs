using System.ComponentModel.DataAnnotations;

namespace ChineseAuction.Models.DTOs;

public class GiftDto
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
    
    public bool IsRaffled { get; set; }
    public string? WinnerName { get; set; }
    public DateTime? RaffleDate { get; set; }
    
    public int DonorId { get; set; }
    public string DonorName { get; set; } = string.Empty;
    
    public int BuyerCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GiftSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsRaffled { get; set; }
    public int BuyerCount { get; set; }
}

public class CreateGiftRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    [Required, Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [Required]
    public int DonorId { get; set; }
}