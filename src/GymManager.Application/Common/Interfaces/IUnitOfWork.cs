namespace GymManager.Application.Common.Interfaces;

/// <summary>
/// Interface para el patrón Unit of Work
/// Coordina las operaciones de múltiples repositorios en una transacción
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Guarda todos los cambios pendientes en la base de datos
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia una transacción explícita
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Confirma la transacción actual
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Revierte la transacción actual
    /// </summary>
    Task RollbackTransactionAsync();
}