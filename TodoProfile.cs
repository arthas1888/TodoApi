using TodoApi.Models.Dto;

namespace TodoApi;

public class TodoProfile : AutoMapper.Profile
{
    public TodoProfile()
    {
        CreateMap<Models.Product, ProductDto>();
    }
}