using System.ComponentModel.DataAnnotations;

namespace ChineseAuction.Models.Entities;

public class Donor
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Gift> Gifts { get; set; } = new List<Gift>();
}