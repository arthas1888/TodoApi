using System.Text.Json.Serialization;

namespace TodoApi.Models.Dto;

public class ProductDto
{
    public required string Name { get; set; }

    [JsonPropertyName("category")]
    public string? CategoryName { get; set; }

    // Additional properties can be added as needed
    
    public static ProductDto FromProduct(Product product)
    {
        return new ProductDto
        {
            Name = product.Name,
            CategoryName = product.CategoryName
        };
    }
}