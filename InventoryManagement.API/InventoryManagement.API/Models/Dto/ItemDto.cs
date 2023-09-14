namespace InventoryManagement.API.Models.Dto;

public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public CategoryDto Category { get; set; }
}