using System.ComponentModel.DataAnnotations;

namespace ChineseAuction.Models.DTOs;

public class DonorDto
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public List<GiftSummaryDto> Gifts { get; set; } = new();
}

public class CreateDonorRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
}