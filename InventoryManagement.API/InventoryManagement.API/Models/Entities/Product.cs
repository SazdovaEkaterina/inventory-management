using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.API.Models.Entities;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(15)]
    public string SerialNumber { get; set; }
    
    public Item Item { get; set; }
    public int ItemId { get; set; }
    
    public InventoryManagementUser? InventoryManagementUser { get; set; }
    public string? InventoryManagementUserId { get; set; }
    
    public bool IsDeleted { get; set; }
}