using InventoryManagement.API.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.DbContexts;

public class InventoryContext : IdentityDbContext<User>
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public new DbSet<User> Users { get; set; } = null!;
    
    public InventoryContext(DbContextOptions<InventoryContext> options)
        :base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)  
    {  
        base.OnModelCreating(builder);  
    }
}