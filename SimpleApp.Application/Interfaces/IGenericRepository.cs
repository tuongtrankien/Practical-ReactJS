using System.Linq.Expressions;
using SimpleApp.Application.Common.Models;

namespace SimpleApp.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<PaginatedResult<TResult>> GetPagedAsync<TResult>(
        Expression<Func<T, bool>>? filter,
        Expression<Func<T, TResult>> selector,
        int pageNumber,
        int pageSize);

    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task Update(T entity);
    Task Delete(Guid id);
    Task SaveChangesAsync();
}