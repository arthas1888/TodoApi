using TodoApi.Models;
using TodoApi.Models.Dto;
using TodoApi.Models.ViewModels;

namespace TodoApi;

public class TodoProfile : AutoMapper.Profile
{
    public TodoProfile()
    {
        CreateMap<Models.Product, ProductDto>();
        CreateMap<ApplicationUser, UserDto>();
    }
}