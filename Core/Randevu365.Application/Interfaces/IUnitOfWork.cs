namespace Randevu365.Application.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IReadRepository<T> GetReadRepository<T>() where T : class;
    IWriteRepository<T> GetWriteRepository<T>() where T : class;
    Task<int> SaveAsync();
    int Save();
}
