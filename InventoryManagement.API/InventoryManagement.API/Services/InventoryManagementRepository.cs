using InventoryManagement.API.DbContexts;
using InventoryManagement.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Services;

public class InventoryManagementRepository : IInventoryManagementRepository
{
    private readonly InventoryContext _context;

    public InventoryManagementRepository(InventoryContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    
    public async Task<(IEnumerable<Item>, PaginationMetadata)> GetItemsAsync(
        string? name, int? categoryId, int pageNumber, int pageSize, 
        int? toQuantity, double? toPrice, int fromQuantity, double fromPrice)
    {
        var totalItems = _context.Items
            .Where(item => !item.IsDeleted
                        && item.Price >= fromPrice)
            .Include(item => item.Products)
            .Where(item => item.Products.Count >= fromQuantity)
            .Include(item => item.Category)
            as IQueryable<Item>;
        
        if (toQuantity != null)
        {
            totalItems = totalItems.Where(item => item.Products.Count <= toQuantity);
        }

        if (toPrice != null)
        {
            totalItems = totalItems.Where(item => item.Price <= toPrice);
        }

        if (categoryId != null)
        {
            totalItems = totalItems.Where(item => item.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim().ToLower();
            totalItems = totalItems.Where(item =>
                item.Name.ToLower().Contains(name));
        }

        var totalItemCount = await totalItems.CountAsync();
        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
        
        var items = await totalItems
            .OrderBy(item => item.Name)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (items, paginationMetadata);
    }

    public async Task<Item?> GetItemAsync(int id)
    {
        return await _context.Items
            .Where(item => item.Id == id && !item.IsDeleted)
            .Include(item => item.Products)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ItemExistsAsync(int id)
    {
        return await _context.Items.AnyAsync(item => item.Id == id);
    }
    
    public void AddItem(Item item)
    {
        _context.Items.Add(item);
    }

    public async Task DeleteItem(Item item)
    {        
        var products = await _context.Products
            .Where(product => product.Item == item)
            .ToListAsync();

        foreach (var product in products)
        {
            this.DeleteProduct(product);
        }
        
        _context.Items.Remove(item);
    }

    public async Task MarkItemAsDeleted(Item item)
    {
        var products = await _context.Products
            .Where(product => product.Item == item)
            .ToListAsync();

        foreach (var product in products)
        {
            this.MarkProductAsDeleted(product);
        }
        
        item.IsDeleted = true;
    }

    public async Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsForUserAsync(string userId, string? serialNumber, int pageNumber, int pageSize)
    {
        var totalProducts = _context.Products
                .Where(product => product.UserId == userId && !product.IsDeleted)
                .Include(product => product.Item)
            as IQueryable<Product>;
                
        if (!string.IsNullOrWhiteSpace(serialNumber))
        {
            serialNumber = serialNumber.Trim().ToLower();
            totalProducts = totalProducts.Where(product =>
                product.SerialNumber.ToLower().Contains(serialNumber));
        }
        
        var totalProductCount = await totalProducts.CountAsync();
        var paginationMetadata = new PaginationMetadata(totalProductCount, pageSize, pageNumber);
        
        var products = await totalProducts
            .OrderBy(product => product.ItemId).ThenBy(product => product.SerialNumber)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (products, paginationMetadata);
    }

    public async Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsAsync(string? serialNumber, int pageNumber, int pageSize, bool filterByAvailability)
    {
        var totalProducts = _context.Products
                .Include(product => product.Item)
            as IQueryable<Product>;

        if (filterByAvailability)
        {
            totalProducts = totalProducts.Where(product => product.UserId == null);
        }
        
        if (!string.IsNullOrWhiteSpace(serialNumber))
        {
            serialNumber = serialNumber.Trim().ToLower();
            totalProducts = totalProducts.Where(product =>
                product.SerialNumber.ToLower().Contains(serialNumber));
        }
        
        var totalProductCount = await totalProducts.CountAsync();
        var paginationMetadata = new PaginationMetadata(totalProductCount, pageSize, pageNumber);
        
        var products = await totalProducts
            .OrderBy(product => product.ItemId).ThenBy(product => product.SerialNumber)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (products, paginationMetadata);
    }

    public async Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsForItemAsync(int itemId, string? serialNumber, int pageNumber, int pageSize)
    {
        var totalProducts = _context.Products
                .Where(product => product.ItemId == itemId && !product.IsDeleted)
            as IQueryable<Product>;
        
        if (!string.IsNullOrWhiteSpace(serialNumber))
        {
            serialNumber = serialNumber.Trim().ToLower();
            totalProducts = totalProducts.Where(product =>
                product.SerialNumber.ToLower().Contains(serialNumber));
        }

        var totalProductCount = await totalProducts.CountAsync();
        var paginationMetadata = new PaginationMetadata(totalProductCount, pageSize, pageNumber);
        
        var products = await totalProducts
            .OrderBy(product => product.SerialNumber)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (products, paginationMetadata);
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        return await _context.Products
            .Where(product => product.Id == id)
            .FirstOrDefaultAsync();
    }

    public void AddProduct(Product product)
    {
        _context.Products.Add(product);
    }

    public void DeleteProduct(Product product)
    {
        _context.Products.Remove(product);
    }

    public void MarkProductAsDeleted(Product product)
    {
        product.IsDeleted = true;
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .OrderBy(category => category.Name)
            .Where(category => !category.IsDeleted)
            .Include(category => category.Items)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryAsync(int id)
    {
        return await _context.Categories
            .Where(category => category.Id == id && !category.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Item>> GetCategoryItems(Category category)
    {
        return await _context.Items
            .OrderBy(item => item.Name)
            .Where(item => item.Category == category && !item.Category.IsDeleted && !item.IsDeleted)
            .ToListAsync();
    }

    public void AddCategory(Category category)
    {
        _context.Categories.Add(category);
    }

    public async Task DeleteCategory(Category category)
    {
        var items = await _context.Items
            .Where(item => item.Category == category)
            .ToListAsync();

        foreach (var item in items)
        {
            await this.DeleteItem(item);
        }
        
        _context.Remove(category);
    }

    public async Task MarkCategoryAsDeleted(Category category)
    {
        var items = await _context.Items
            .Where(item => item.Category == category)
            .ToListAsync();

        foreach (var item in items)
        {
            await this.MarkItemAsDeleted(item);
        }

        category.IsDeleted = true;
    }

}