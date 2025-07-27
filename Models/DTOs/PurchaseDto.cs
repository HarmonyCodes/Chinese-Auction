namespace ChineseAuction.Models.DTOs;

public class PurchaseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPhone { get; set; } = string.Empty;
    
    public int GiftId { get; set; }
    public string GiftName { get; set; } = string.Empty;
    public string GiftCategory { get; set; } = string.Empty;
    public string DonorName { get; set; } = string.Empty;
    
    public decimal PricePaid { get; set; }
    public DateTime PurchaseDate { get; set; }
}