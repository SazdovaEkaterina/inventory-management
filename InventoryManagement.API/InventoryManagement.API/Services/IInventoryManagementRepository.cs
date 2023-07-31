using InventoryManagement.API.Models.Entities;

namespace InventoryManagement.API.Services;

public interface IInventoryManagementRepository
{
    
    Task<bool> SaveChangesAsync();

    Task<(IEnumerable<Item>, PaginationMetadata)> GetItemsAsync(string? name, int? categoryId, int pageNumber, int pageSize, 
        int? toQuantity, double? toPrice, int fromQuantity, double fromPrice);
    Task<Item?> GetItemAsync(int id);
    void AddItem(Item item);
    void DeleteItem(Item item);
    void MarkItemAsDeleted(Item item);
    
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryAsync(int id);
    Task<IEnumerable<Item>> GetCategoryItems(Category category);
    void AddCategory(Category category);
    Task DeleteCategory(Category category);
    Task MarkCategoryAsDeleted(Category category);
}