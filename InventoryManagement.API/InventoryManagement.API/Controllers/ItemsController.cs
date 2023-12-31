using System.Text.Json;
using AutoMapper;
using InventoryManagement.API.Models.Dto;
using InventoryManagement.API.Models.Entities;
using InventoryManagement.API.Services;
using InventoryManagement.API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
//[Authorize(AuthenticationSchemes = "Bearer")]
public class ItemsController : ControllerBase
{
    private readonly IInventoryManagementRepository _inventoryManagementRepository;
    private readonly IMapper _mapper;
    private const int MaxItemsPageSize = 10;
    
    public ItemsController(
        IInventoryManagementRepository inventoryManagementRepository,
        IMapper mapper)
    {
        _inventoryManagementRepository = inventoryManagementRepository ??
                                         throw new ArgumentNullException(nameof(inventoryManagementRepository));
        _mapper = mapper;
    }
    
    [HttpGet("api/items")]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] int? categoryId,
        [FromQuery] int? toQuantity,
        [FromQuery] double? toPrice,
        [FromQuery] int fromQuantity = 0,
        [FromQuery] double fromPrice = 0,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageSize > MaxItemsPageSize)
        {
            pageSize = MaxItemsPageSize;
        }

        var (itemList, paginationMetadata) = 
            await _inventoryManagementRepository
            .GetItemsAsync(name, categoryId, pageNumber, pageSize, 
                toQuantity, toPrice, fromQuantity, fromPrice);
        
        Response.Headers.Add("X-Pagination", 
            JsonSerializer.Serialize(paginationMetadata));
        
        var itemDtoList = _mapper.Map<IEnumerable<ItemDto>>(itemList);
        return Ok(itemDtoList);
    }

    [HttpGet("api/items/{id}", Name = "GetItem")]
    public async Task<ActionResult<ItemDto>> Get(
        [FromRoute] int id)
    {
        var item = await _inventoryManagementRepository.GetItemAsync(id);
        
        if (item == null)
        {
            return NotFound();
        }

        var itemDto = _mapper.Map<ItemDto>(item);
        
        return Ok(itemDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/items/add")]
    public async Task<ActionResult<ItemDto>> Create(
        [FromBody] ItemDto itemDto)
    {
        var item = _mapper.Map<Item>(itemDto);
        
        _inventoryManagementRepository.AddItem(item);
        await _inventoryManagementRepository.SaveChangesAsync();
        
        return CreatedAtRoute("GetItem",
            new
            {
                id = item.Id
            },
            _mapper.Map<ItemDto>(item));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("api/items/{id}/edit")]
    public async Task<ActionResult> Update(
        [FromRoute] int id, 
        [FromBody] ItemDto itemDto)
    {
        var item = await _inventoryManagementRepository.GetItemAsync(id);
        
        if (item == null)
        {
            return NotFound();
        }

        _mapper.Map(itemDto, item);
        await _inventoryManagementRepository.SaveChangesAsync();

        return Ok(_mapper.Map<ItemDto>(item));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("api/items/{id}/partial-edit")]
    public async Task<ActionResult> PartiallyUpdate(
        [FromRoute] int id, 
        [FromBody] JsonPatchDocument<ItemDto> patchDocument)
    {
        var item = await _inventoryManagementRepository.GetItemAsync(id);
        
        if (item == null)
        {
            return NotFound();
        }

        var itemDto = _mapper.Map<ItemDto>(item);
        
        patchDocument.ApplyTo(itemDto, ModelState);    
        
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }    
        
        if (!TryValidateModel(itemDto))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(itemDto, item);

        await _inventoryManagementRepository.SaveChangesAsync();

        return Ok(_mapper.Map<ItemDto>(item));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/items/{id}/delete")]
    public async Task<ActionResult> HardDelete(
        [FromRoute] int id)
    {
        var item = await _inventoryManagementRepository.GetItemAsync(id);
        
        if (item == null)
        {
            return NotFound();
        }
        
        await _inventoryManagementRepository.DeleteItem(item);
        await _inventoryManagementRepository.SaveChangesAsync();

        return NoContent();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("api/items/{id}/delete")]
    public async Task<ActionResult> SoftDelete(
        [FromRoute] int id)
    {
        var item = await _inventoryManagementRepository.GetItemAsync(id);
        
        if (item == null)
        {
            return NotFound();
        }
        
        await _inventoryManagementRepository.MarkItemAsDeleted(item);
        await _inventoryManagementRepository.SaveChangesAsync();

        return NoContent();
    }
}