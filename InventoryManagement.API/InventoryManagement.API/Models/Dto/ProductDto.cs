namespace InventoryManagement.API.Models.Dto;

public class ProductDto
{
    public string SerialNumber { get; set; }
    public int ItemId { get; set; }
    public string? UserId { get; set; }
}