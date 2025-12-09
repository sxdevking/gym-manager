using System.Linq.Expressions;

namespace GymManager.Application.Common.Interfaces;

/// <summary>
/// Interface genérica de repositorio para operaciones CRUD
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public interface IRepository<T> where T : class
{
    // ═══════════════════════════════════════════════════════════
    // CONSULTAS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Obtiene una entidad por su ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtiene todas las entidades
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene entidades que cumplan con un predicado
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Obtiene la primera entidad que cumpla con un predicado
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla con un predicado
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Cuenta las entidades que cumplen con un predicado
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    // ═══════════════════════════════════════════════════════════
    // COMANDOS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Agrega una nueva entidad
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Agrega múltiples entidades
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Actualiza múltiples entidades
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Elimina una entidad
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Elimina múltiples entidades
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);

    // ═══════════════════════════════════════════════════════════
    // CONSULTAS AVANZADAS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Obtiene un IQueryable para consultas personalizadas
    /// </summary>
    IQueryable<T> Query();

    /// <summary>
    /// Obtiene entidades con includes para carga eager
    /// </summary>
    Task<IEnumerable<T>> GetWithIncludesAsync(
        Expression<Func<T, bool>>? predicate = null,
        params Expression<Func<T, object>>[] includes);
}