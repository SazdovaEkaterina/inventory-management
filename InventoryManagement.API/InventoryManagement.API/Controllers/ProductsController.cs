using System.Text.Json;
using AutoMapper;
using InventoryManagement.API.Models.Dto;
using InventoryManagement.API.Models.Entities;
using InventoryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ProductsController : ControllerBase
{
    private readonly IInventoryManagementRepository _inventoryManagementRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private const int MaxProductsPageSize = 10;

    public ProductsController(
        IInventoryManagementRepository inventoryManagementRepository,
        UserManager<User> userManager, 
        IMapper mapper)
    {
        _inventoryManagementRepository = inventoryManagementRepository ??
                                         throw new ArgumentNullException(nameof(inventoryManagementRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper;
    }
    
    [HttpGet("api/products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromQuery] string? serialNumber,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterByAvailability = "false")
    {
        if (pageSize > MaxProductsPageSize)
        {
            pageSize = MaxProductsPageSize;
        }

        var filter = bool.Parse(filterByAvailability ?? "false");
        
        var (productList, paginationMetadata) = 
            await _inventoryManagementRepository
                .GetProductsAsync(serialNumber, pageNumber, pageSize, filter);
        
        Response.Headers.Add("X-Pagination", 
            JsonSerializer.Serialize(paginationMetadata));
        
        var productDtoList = _mapper.Map<IEnumerable<ProductDto>>(productList);
        return Ok(productDtoList);
    }
    
    [HttpGet("api/users/{userId}/inventory")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetInventory(
        [FromRoute] string userId,
        [FromQuery] string? serialNumber,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        
        if (pageSize > MaxProductsPageSize)
        {
            pageSize = MaxProductsPageSize;
        }
        
        var (productList, paginationMetadata) = 
            await _inventoryManagementRepository
                .GetProductsForUserAsync(userId, serialNumber, pageNumber, pageSize);
        
        Response.Headers.Add("X-Pagination", 
            JsonSerializer.Serialize(paginationMetadata));
        
        var productDtoList = _mapper.Map<IEnumerable<ProductDto>>(productList);
        return Ok(productDtoList);
    }

    [HttpGet("api/items/{itemId}/products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllForItem(
        [FromRoute] int itemId,
        [FromQuery] string? serialNumber,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (!await _inventoryManagementRepository.ItemExistsAsync(itemId))
        {
            return NotFound();
        }
        
        if (pageSize > MaxProductsPageSize)
        {
            pageSize = MaxProductsPageSize;
        }
        
        var (productList, paginationMetadata) = 
            await _inventoryManagementRepository
                .GetProductsForItemAsync(itemId, serialNumber, pageNumber, pageSize);
        
        Response.Headers.Add("X-Pagination", 
            JsonSerializer.Serialize(paginationMetadata));
        
        var productDtoList = _mapper.Map<IEnumerable<ProductDto>>(productList);
        return Ok(productDtoList);
    }

    [HttpGet("api/items/{itemId}/products/{id}", Name = "GetProduct")]
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

    [HttpPost("api/items/{itemId}/products/add")]
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
            (itemId, id: product.Id),
            _mapper.Map<ProductDto>(product));
    }

    [HttpPut("api/items/{itemId}/products/{id}/edit")]
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

    [HttpPatch("api/items/{itemId}/products/{id}/partial-edit")]
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

    [HttpDelete("api/items/{itemId}/products/{id}/delete")]
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

    [HttpPost("api/items/{itemId}/products/{id}/delete")]
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