using AutoMapper;
using InventoryManagement.API.Models.Dto;
using InventoryManagement.API.Models.Entities;
using InventoryManagement.API.Services;
using InventoryManagement.API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class CategoriesController : ControllerBase
{
    private readonly IInventoryManagementRepository _inventoryManagementRepository;
    private readonly IMapper _mapper;

    public CategoriesController(
        IInventoryManagementRepository inventoryManagementRepository,
        IMapper mapper)
    {
        _inventoryManagementRepository = inventoryManagementRepository ??
                                         throw new ArgumentNullException(nameof(inventoryManagementRepository));
        _mapper = mapper;
    }
    
    [HttpGet("api/categories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categoryList = await _inventoryManagementRepository.GetCategoriesAsync();
        var categoryDtoList = _mapper.Map<IEnumerable<CategoryDto>>(categoryList);
        return Ok(categoryDtoList);
    }

    [HttpGet("api/categories/{id}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDto>> Get(
        [FromRoute] int id)
    {
        var category = await _inventoryManagementRepository.GetCategoryAsync(id);

        if (category == null)
        {
            return NotFound();
        }
        
        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Ok(categoryDto);
    }

    [HttpGet("api/categories/{id}/items")]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems(
        [FromRoute] int id)
    {
        var category = await _inventoryManagementRepository.GetCategoryAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        var itemList = await _inventoryManagementRepository.GetCategoryItems(category);
        var itemDtoList = _mapper.Map<IEnumerable<ItemDto>>(itemList);
        return Ok(itemDtoList);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/categories/add")]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CategoryDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);
        
        _inventoryManagementRepository.AddCategory(category);
        await _inventoryManagementRepository.SaveChangesAsync();
        
        return CreatedAtRoute("GetCategory",
            new
            {
                id = category.Id
            },
            categoryDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/categories/{id}/edit")]
    public async Task<ActionResult> Update(
        [FromRoute] int id, 
        [FromBody] CategoryDto categoryDto)
    {
        var category = await _inventoryManagementRepository.GetCategoryAsync(id);
        
        if (category == null)
        {
            return NotFound();
        }

        _mapper.Map(categoryDto, category);
        await _inventoryManagementRepository.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("api/categories/{id}/partial-edit")]
    public async Task<ActionResult> PartiallyUpdate(
        [FromRoute] int id, 
        [FromBody] JsonPatchDocument<CategoryDto> patchDocument)
    {
        var category = await _inventoryManagementRepository.GetCategoryAsync(id);
        
        if (category == null)
        {
            return NotFound();
        }

        var categoryDto = _mapper.Map<CategoryDto>(category);
        
        patchDocument.ApplyTo(categoryDto, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
                
        if (!TryValidateModel(categoryDto))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(categoryDto, category);

        await _inventoryManagementRepository.SaveChangesAsync();

        return NoContent();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("api/categories/{id}/delete")]
    public async Task<ActionResult> HardDelete(
        [FromRoute] int id)
    {
        var category = await _inventoryManagementRepository.GetCategoryAsync(id);
        
        if (category == null)
        {
            return NotFound();
        }
        
        await _inventoryManagementRepository.DeleteCategory(category);
        await _inventoryManagementRepository.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/categories/{id}/delete")]
    public async Task<ActionResult> SoftDelete(
        [FromRoute] int id)
    {
        var category = await _inventoryManagementRepository.GetCategoryAsync(id);
        
        if (category == null)
        {
            return NotFound();
        }
        
        await _inventoryManagementRepository.MarkCategoryAsDeleted(category);
        await _inventoryManagementRepository.SaveChangesAsync();

        return NoContent();
    }
}