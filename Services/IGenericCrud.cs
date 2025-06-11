using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;


public interface IGenericCrud<T> where T : BaseModel
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(int id, T entity);
    Task<T?> DeleteAsync(int id);
    bool Exists(int id);
}

public class GenericCrudService<T>(ApplicationDbContext dbContext) : IGenericCrud<T> where T : BaseModel
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> UpdateAsync(int id, T entity)
    {
        if (!Exists(id)) return null;
        // throw new KeyNotFoundException($"Entity with id {id} not found.");

        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return null;

        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public bool Exists(int id)
    {
        //  _dbContext.todos.Any(e => e.Id == id);
        return _dbContext.Set<T>().Any(e => EF.Property<int>(e, "id") == id);
    }
}