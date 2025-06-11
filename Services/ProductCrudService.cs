using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public class ProductCrudService(ApplicationDbContext dbContext) : GenericCrudService<Product>(dbContext)
{

    public readonly ApplicationDbContext _dbContext = dbContext;

    override
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbContext.Set<Product>()
            .Include(p => p.Category) // Include related Category data
            .ToListAsync();
    }

    override
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<Product>()
            .Include(p => p.Category) // Include related Category data
            .FirstOrDefaultAsync(x => x.Id == id);
    }

}