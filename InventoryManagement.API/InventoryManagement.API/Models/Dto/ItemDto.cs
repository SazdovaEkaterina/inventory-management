namespace InventoryManagement.API.Models.Dto;

public class ItemDto
{
    public string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public bool IsDeleted { get; set; }
}