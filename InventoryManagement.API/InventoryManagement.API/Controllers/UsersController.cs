using System.Text.Json;
using AutoMapper;
using InventoryManagement.API.Models.Dto;
using InventoryManagement.API.Models.Entities;
using InventoryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/users/{id}")]
public class UsersController : ControllerBase
{
    private readonly IInventoryManagementRepository _inventoryManagementRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private const int MaxProductsPageSize = 20;

    public UsersController(
        IInventoryManagementRepository inventoryManagementRepository,
        UserManager<User> userManager, 
        IMapper mapper)
    {
        _inventoryManagementRepository = inventoryManagementRepository ??
                                         throw new ArgumentNullException(nameof(inventoryManagementRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper;
    }

    [HttpGet("inventory")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetInventory(
        [FromRoute] string id,
        [FromQuery] string? serialNumber,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var user = await _userManager.FindByIdAsync(id);
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
                .GetProductsForUserAsync(id, serialNumber, pageNumber, pageSize);
        
        Response.Headers.Add("X-Pagination", 
            JsonSerializer.Serialize(paginationMetadata));
        
        var productDtoList = _mapper.Map<IEnumerable<ProductDto>>(productList);
        return Ok(productDtoList);
    }
}