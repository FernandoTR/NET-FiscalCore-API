using FiscalCore.Application.Abstractions;
using FiscalCore.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace FiscalCore.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly FiscalCoreDbContext _context;
    private IDbContextTransaction? _transaction;

    public EfUnitOfWork(FiscalCoreDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already started.");

        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction.");

        await _context.SaveChangesAsync(ct);
        await _transaction.CommitAsync(ct);

        await DisposeTransactionAsync();
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction == null)
            return;

        await _transaction.RollbackAsync(ct);
        await DisposeTransactionAsync();
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
            await _transaction.DisposeAsync();
    }
}


