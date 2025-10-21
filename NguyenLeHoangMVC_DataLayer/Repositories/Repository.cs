// DataLayer/Repositories/Repository.cs
using NguyenLeHoangMVC_DataLayer.Models;  // CHANGE: Import Models.
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;  // CHANGE: Add using for Expression<Func<T, bool>> (fix CS0246).

namespace NguyenLeHoangMVC_DataLayer.Repositories;  // CHANGE: Update namespace to match project name.

public class Repository<T> : IRepository<T> where T : class {
    protected readonly FunewsManagementContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(FunewsManagementContext context) {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> GetByIdAsync(object id) {
        // CHANGE: Use ?? null to fix warning CS8603 (possible null return).
        return await _dbSet.FindAsync(id) ?? null;
    }

    public async Task<T> InsertAsync(T entity) {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity) {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(object id) {
        var entity = await GetByIdAsync(id);
        if (entity != null) {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) {
        // CHANGE: Implement correctly with Where(predicate) to match interface (fix CS0535).
        return await _dbSet.Where(predicate).ToListAsync();  // CHANGE: LINQ cho search (ví dụ: Where(x => x.NewsStatus == 1)).
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize) {
        return await _dbSet.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}