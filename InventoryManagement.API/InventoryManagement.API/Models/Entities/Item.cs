using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.API.Models.Entities;

public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }
    public bool IsDeleted { get; set; }
}