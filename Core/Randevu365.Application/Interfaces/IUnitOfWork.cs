using Randevu365.Domain.Base;

namespace Randevu365.Application.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IReadRepository<T> GetReadRepository<T>() where T : class;
    IWriteRepository<T> GetWriteRepository<T>() where T : class;
    Task<int> SaveAsync();
    int Save();
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
