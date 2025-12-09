using System.Linq.Expressions;
using GymManager.Application.Common.Interfaces;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Infrastructure.Repositories;

/// <summary>
/// Implementación genérica del patrón Repository
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly GymDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(GymDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // ═══════════════════════════════════════════════════════════
    // CONSULTAS
    // ═══════════════════════════════════════════════════════════

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.CountAsync();

        return await _dbSet.CountAsync(predicate);
    }

    // ═══════════════════════════════════════════════════════════
    // COMANDOS
    // ═══════════════════════════════════════════════════════════

    public virtual async Task<T> AddAsync(T entity)
    {
        var entry = await _dbSet.AddAsync(entity);
        return entry.Entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Update(entity);
        }
    }

    public virtual void Remove(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    // ═══════════════════════════════════════════════════════════
    // CONSULTAS AVANZADAS
    // ═══════════════════════════════════════════════════════════

    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public virtual async Task<IEnumerable<T>> GetWithIncludesAsync(
        Expression<Func<T, bool>>? predicate = null,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        // Aplicar includes
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Aplicar filtro si existe
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }
}