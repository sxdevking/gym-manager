using GymManager.Application.Common.Interfaces;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace GymManager.Infrastructure.Repositories;

/// <summary>
/// Implementación del patrón Unit of Work
/// Coordina las operaciones de múltiples repositorios
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly GymDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(GymDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Guarda todos los cambios pendientes en la base de datos
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Inicia una transacción explícita
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Ya existe una transacción activa.");
        }

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Confirma la transacción actual
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No hay una transacción activa para confirmar.");
        }

        try
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <summary>
    /// Revierte la transacción actual
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No hay una transacción activa para revertir.");
        }

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <summary>
    /// Libera los recursos de la transacción
    /// </summary>
    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Libera los recursos del UnitOfWork
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}