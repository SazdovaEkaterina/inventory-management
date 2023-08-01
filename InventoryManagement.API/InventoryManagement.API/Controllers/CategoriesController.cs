using AutoMapper;
using InventoryManagement.API.Models.Dto;
using InventoryManagement.API.Models.Entities;
using InventoryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/categories")]
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
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categoryList = await _inventoryManagementRepository.GetCategoriesAsync();
        var categoryDtoList = _mapper.Map<IEnumerable<CategoryDto>>(categoryList);
        return Ok(categoryDtoList);
    }

    [HttpGet("{id}", Name = "GetCategory")]
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

    [HttpGet("{id}/items")]
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

    [HttpPost("add")]
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

    [HttpPut("{id}/edit")]
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

    [HttpPatch("{id}/partial-edit")]
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
    
    [HttpDelete("{id}/delete")]
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

    [HttpPost("{id}/delete")]
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