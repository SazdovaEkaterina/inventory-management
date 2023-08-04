using InventoryManagement.API.Models.Entities;

namespace InventoryManagement.API.Services.IServices;

public interface IInventoryManagementRepository
{
    
    Task<bool> SaveChangesAsync();

    Task<(IEnumerable<Item>, PaginationMetadata)> GetItemsAsync(string? name, int? categoryId, int pageNumber, int pageSize, 
        int? toQuantity, double? toPrice, int fromQuantity, double fromPrice);
    Task<Item?> GetItemAsync(int id);
    Task<bool> ItemExistsAsync(int id);
    void AddItem(Item item);
    Task DeleteItem(Item item);
    Task MarkItemAsDeleted(Item item);
    
    Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsForUserAsync(string userId, string? serialNumber, int pageNumber, int pageSize);
    Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsAsync(string? serialNumber, int pageNumber, int pageSize, bool filterByAvailability);
    
    Task<(IEnumerable<Product>, PaginationMetadata)> GetProductsForItemAsync(int itemId, string? serialNumber, int pageNumber, int pageSize);
    Task<Product?> GetProductAsync(int id);
    void AddProduct(Product product);
    void DeleteProduct(Product product);
    void MarkProductAsDeleted(Product product);
    
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryAsync(int id);
    Task<IEnumerable<Item>> GetCategoryItems(Category category);
    void AddCategory(Category category);
    Task DeleteCategory(Category category);
    Task MarkCategoryAsDeleted(Category category);
}