using System.Text.Json;
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
[Route("api/items/{itemId}/products")]
public class ProductsController : ControllerBase
{
    private readonly IInventoryManagementRepository _inventoryManagementRepository;
    private readonly IMapper _mapper;
    private const int MaxItemsPageSize = 10;

    public ProductsController(IInventoryManagementRepository inventoryManagementRepository, IMapper mapper)
    {
        _inventoryManagementRepository = inventoryManagementRepository ??
                                         throw new ArgumentNullException(nameof(inventoryManagementRepository));
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromRoute] int itemId,
        [FromQuery] string? serialNumber,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (!await _inventoryManagementRepository.ItemExistsAsync(itemId))
        {
            return NotFound();
        }
        
        if (pageSize > MaxItemsPageSize)
        {
            pageSize = MaxItemsPageSize;
        }
        
        var (productList, paginationMetadata) = 
            await _inventoryManagementRepository
                .GetProductsAsync(itemId, serialNumber, pageNumber, pageSize);
        
        Response.Headers.Add("X-Pagination", 
            JsonSerializer.Serialize(paginationMetadata));
        
        var productDtoList = _mapper.Map<IEnumerable<ProductDto>>(productList);
        return Ok(productDtoList);
    }

    [HttpGet("{id}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDto>> Get(
        [FromRoute] int itemId,
        [FromRoute] int id)
    {
        if (!await _inventoryManagementRepository.ItemExistsAsync(itemId))
        {
            return NotFound();
        }
        
        var product = await _inventoryManagementRepository.GetProductAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var productDto = _mapper.Map<ProductDto>(product);
        return Ok(productDto);
    }

    [HttpPost("add")]
    public async Task<ActionResult<ProductDto>> Create(
        [FromRoute] int itemId,
        [FromBody] ProductDto productDto)
    {
        if (!await _inventoryManagementRepository.ItemExistsAsync(itemId))
        {
            return NotFound();
        }

        productDto.ItemId = itemId;
        var product = _mapper.Map<Product>(productDto);

        _inventoryManagementRepository.AddProduct(product);
        await _inventoryManagementRepository.SaveChangesAsync();

        return CreatedAtRoute("GetProduct",
            new
            {
                itemId = itemId,
                id = product.Id
            },
            _mapper.Map<ProductDto>(product));
    }

    [HttpPut("{id}/edit")]
    public async Task<ActionResult> Update(
        [FromRoute] int itemId,
        [FromRoute] int id,
        [FromBody] ProductDto productDto)
    {
        if (!await _inventoryManagementRepository.ItemExistsAsync(itemId))
        {
            return NotFound();
        }
        
        var product = await _inventoryManagementRepository.GetProductAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        _mapper.Map(productDto, product);
        await _inventoryManagementRepository.SaveChangesAsync();

        return Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpPatch("{id}/partial-edit")]
    public async Task<ActionResult> PartiallyUpdate(
        [FromRoute] int itemId,
        [FromRoute] int id, 
        [FromBody] JsonPatchDocument<ProductDto> patchDocument)
    {
        if (!await _inventoryManagementRepository.ItemExistsAsync(itemId))
        {
            return NotFound();
        }
        
        var product = await _inventoryManagementRepository.GetProductAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var productDto = _mapper.Map<ProductDto>(product);
        
        patchDocument.ApplyTo(productDto, ModelState);  
        
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (!TryValidateModel(productDto))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(productDto, product);

        await _inventoryManagementRepository.SaveChangesAsync();

        return Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpDelete("{id}/delete")]
    public async Task<ActionResult> HardDelete(
        [FromRoute] int itemId,
        [FromRoute] int id)
    {
        if (!await _inventoryManagementRepository.ItemExistsAsync(itemId))
        {
            return NotFound();
        }
        
        var product = await _inventoryManagementRepository.GetProductAsync(id);

        if (product == null)
        {
            return NotFound();
        }
        
        _inventoryManagementRepository.DeleteProduct(product);
        await _inventoryManagementRepository.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPost("{id}/delete")]
    public async Task<ActionResult> SoftDelete(
        [FromRoute] int itemId,
        [FromRoute] int id)
    {
        if (!await _inventoryManagementRepository.ItemExistsAsync(itemId))
        {
            return NotFound();
        }
        
        var product = await _inventoryManagementRepository.GetProductAsync(id);

        if (product == null)
        {
            return NotFound();
        }
        
        _inventoryManagementRepository.MarkProductAsDeleted(product);
        await _inventoryManagementRepository.SaveChangesAsync();

        return NoContent();
    }
}