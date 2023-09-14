namespace InventoryManagement.API.Models.Dto;

public class ProductDto
{
    public int Id { get; set; }
    public string SerialNumber { get; set; }
    public int ItemId { get; set; }
    public string? UserId { get; set; }
}