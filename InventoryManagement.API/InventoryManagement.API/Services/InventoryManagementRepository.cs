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
                        && item.Quantity >= fromQuantity
                        && item.Price >= fromPrice)
            .Include(item => item.Category)
            as IQueryable<Item>;

        if (toQuantity != null)
        {
            totalItems = totalItems.Where(item => item.Quantity <= toQuantity);
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
            .FirstOrDefaultAsync();
    }

    public void AddItem(Item item)
    {
        _context.Items.Add(item);
    }

    public void DeleteItem(Item item)
    {
        _context.Items.Remove(item);
    }

    public void MarkItemAsDeleted(Item item)
    {
        item.IsDeleted = true;
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
            _context.Remove(item);
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
            item.IsDeleted = true;
        }

        category.IsDeleted = true;
    }
}