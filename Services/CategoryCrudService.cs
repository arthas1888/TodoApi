using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public class CategoryCrudService(ApplicationDbContext dbContext) : GenericCrudService<Category>(dbContext)
{

    public readonly ApplicationDbContext _dbContext = dbContext;

    override
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbContext.Set<Category>()
            .Include(p => p.Products) // Include related Category data
            .ToListAsync();
    }

    override
    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<Category>()
            .Include(p => p.Products) // Include related Category data
            .FirstOrDefaultAsync(x => x.Id == id);
    }

}