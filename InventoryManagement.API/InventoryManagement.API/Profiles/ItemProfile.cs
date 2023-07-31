using AutoMapper;
using InventoryManagement.API.Models.Dto;
using InventoryManagement.API.Models.Entities;

namespace InventoryManagement.API.Profiles;

public class ItemProfile : Profile
{
    public ItemProfile()
    {
        CreateMap<Item, ItemDto>();
        CreateMap<ItemDto, Item>();
    }
}