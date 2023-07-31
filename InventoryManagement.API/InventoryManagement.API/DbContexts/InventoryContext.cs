using InventoryManagement.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.DbContexts;

public class InventoryContext : DbContext
{
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<InventoryManagementUser> InventoryManagementUsers { get; set; } = null!;
    
    public InventoryContext(DbContextOptions<InventoryContext> options)
        :base(options)
    {
        
    }
}