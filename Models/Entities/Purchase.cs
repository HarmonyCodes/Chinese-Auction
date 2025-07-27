namespace ChineseAuction.Models.Entities;

public class Purchase
{
    public int Id { get; set; }
    
    // Foreign keys
    public int UserId { get; set; }
    public int GiftId { get; set; }
    
    public decimal PricePaid { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Gift Gift { get; set; } = null!;
}