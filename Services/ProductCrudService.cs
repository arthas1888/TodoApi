
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public class ProductCrudService(ApplicationDbContext dbContext, PostgresService postgresService, ILogger<ProductCrudService> logger) : GenericCrudService<Product>(dbContext)
{

    public readonly ApplicationDbContext _dbContext = dbContext;
    public readonly PostgresService _postgresService = postgresService;
    public readonly ILogger<ProductCrudService> _logger = logger;

    override
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        // _dbContext.Set<Product> () => _dbContext.product;

        IQueryable<Product> products = _dbContext.product
           .AsNoTracking() // Use AsNoTracking for read-only queries
           .Include(x => x.Category);

        products = products.Where(p => p.Category != null); // Filter out products without a category
        var results = await products
            .ToListAsync();


        var categories = await _dbContext.category
            .AsNoTracking()
            .ToListAsync();

        // en masa
        // await _dbContext.product.ExecuteUpdateAsync(x => x.SetProperty(p => p.Name, p => p.Name.ToLower())); // Set Category to null for all products
        // var categoryNames = categories.Select(c => c.Name).ToList();
        foreach (var product in results)
        {
            product.Name = product.Name.ToUpperInvariant(); // Example transformation
            // _logger.LogInformation("Product: {Id}, Name: {Name}, Category: {CategoryName}",
            //      product.Id, product.Name, product.Category?.Name);
        }

        // await _dbContext.SaveChangesAsync();
        await TestUpdate();

        return await _dbContext.Set<Product>()
            .Include(p => p.Category) // Include related Category data           
            .ToListAsync();
    }

    private async Task TestUpdate()
    {


        var product = new Product
        {
            Name = "Mandarina",
            CategoryId = 1 // Assuming a category with ID 1 exists
        };
        
        // product.Id = 100;
        // change. tracking
        _dbContext.Entry(product).State = EntityState.Added;
        await _dbContext.SaveChangesAsync();
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