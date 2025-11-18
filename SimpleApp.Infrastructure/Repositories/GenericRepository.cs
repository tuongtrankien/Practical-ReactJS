using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleApp.Application.Common.Models;
using SimpleApp.Application.Interfaces;
using SimpleApp.Infrastructure.Data;

namespace SimpleApp.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector)
    {
        return await _dbSet.AsNoTracking().Select(selector).ToListAsync();
    }

    public async Task<PaginatedResult<TResult>> GetPagedAsync<TResult>(
        Expression<Func<T, bool>>? filter,
        Expression<Func<T, TResult>> selector,
        int pageNumber,
        int pageSize)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync();

        return new PaginatedResult<TResult>(items, totalItems, pageNumber, pageSize);
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task Update(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task Delete(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}