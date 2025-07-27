namespace ChineseAuction.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public bool IsDraft { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int GiftId { get; set; }
    public string GiftName { get; set; } = string.Empty;
    public string GiftCategory { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsDraft { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AddToCartRequest
{
    [Required]
    public int GiftId { get; set; }
}