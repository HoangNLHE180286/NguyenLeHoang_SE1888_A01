using System.Linq.Expressions;  // CHANGE: Add using for Expression<Func<T, bool>> (fix potential issues).

namespace NguyenLeHoangMVC_DataLayer.Repositories;  // CHANGE: Update namespace to match project name.

public interface IRepository<T> where T : class {
    // CHANGE: Generic CRUD + LINQ cho search/sort (khớp đề).
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(object id);
    Task<T> InsertAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(object id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);  // Cho search.
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);  // Cho sort/paging nếu cần.
}