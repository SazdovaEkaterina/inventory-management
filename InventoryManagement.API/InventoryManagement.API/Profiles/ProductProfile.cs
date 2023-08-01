using AutoMapper;
using InventoryManagement.API.Models.Dto;
using InventoryManagement.API.Models.Entities;

namespace InventoryManagement.API.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();
    }
}