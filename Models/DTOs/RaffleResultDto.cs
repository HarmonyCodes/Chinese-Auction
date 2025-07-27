namespace ChineseAuction.Models.DTOs;

public class RaffleResultDto
{
    public int GiftId { get; set; }
    public string GiftName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string DonorName { get; set; } = string.Empty;
    
    public int? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public string? WinnerEmail { get; set; }
    public string? WinnerPhone { get; set; }
    
    public DateTime RaffleDate { get; set; }
    public int TotalParticipants { get; set; }
}

public class SalesReportDto
{
    public decimal TotalIncome { get; set; }
    public int TotalPurchases { get; set; }
    public int TotalCustomers { get; set; }
    public List<CategorySalesDto> CategoryBreakdown { get; set; } = new();
    public List<TopGiftDto> TopGifts { get; set; } = new();
}

public class CategorySalesDto
{
    public string Category { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int ItemCount { get; set; }
}

public class TopGiftDto
{
    public int GiftId { get; set; }
    public string GiftName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int PurchaseCount { get; set; }
    public decimal TotalRevenue { get; set; }
}