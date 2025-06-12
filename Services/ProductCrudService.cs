using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public class ProductCrudService(ApplicationDbContext dbContext, PostgresService postgresService) : GenericCrudService<Product>(dbContext)
{

    public readonly ApplicationDbContext _dbContext = dbContext;
    public readonly PostgresService _postgresService = postgresService;

    override
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbContext.Set<Product>()
            .Include(p => p.Category) // Include related Category data           
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> SelectGetAllAsync()
    {
        return await _dbContext.Set<Product>()
            .Include(p => p.Category) // Include related Category data
            .Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category.Name
            })
            .ToListAsync();
    }

    public async Task<string> GetSqlAllAsync()
    {
        var query = "SELECT p.id, p.name, c.name as category FROM product p LEFT JOIN category c ON p.category_id = c.id";
        return await _postgresService.FetchAsync(query);
    }

    override
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<Product>()
            .Include(p => p.Category) // Include related Category data
            .FirstOrDefaultAsync(x => x.Id == id);
    }

}