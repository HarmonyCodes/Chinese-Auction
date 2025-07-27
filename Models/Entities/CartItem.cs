namespace ChineseAuction.Models.Entities;

public class CartItem
{
    public int Id { get; set; }
    
    // Foreign keys
    public int UserId { get; set; }
    public int GiftId { get; set; }
    
    public bool IsDraft { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Gift Gift { get; set; } = null!;
}