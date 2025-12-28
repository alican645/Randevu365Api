using Randevu365.Application.Interfaces;
using Randevu365.Persistence.Context;
using Randevu365.Persistence.Repositories;

namespace Randevu365.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();
        

    public int Save() => dbContext.SaveChanges();
    public async Task<int> SaveAsync() => await dbContext.SaveChangesAsync();
    IReadRepository<T> IUnitOfWork.GetReadRepository<T>() => new ReadRepository<T>(dbContext);
    IWriteRepository<T> IUnitOfWork.GetWriteRepository<T>() => new WriteRepository<T>(dbContext);
}