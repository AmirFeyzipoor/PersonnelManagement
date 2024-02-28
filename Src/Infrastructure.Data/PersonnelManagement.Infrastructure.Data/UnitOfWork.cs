using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;

    public UnitOfWork(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task Begin()
    {
        await _dataContext.Database.BeginTransactionAsync();
    }

    public async Task Commit()
    {
        await _dataContext.SaveChangesAsync();
        await _dataContext.Database.CommitTransactionAsync();
    }

    public async Task Rollback()
    {
        await _dataContext.Database.RollbackTransactionAsync();
    }

    public async Task Complete()
    {
        await _dataContext.SaveChangesAsync();
    }
}