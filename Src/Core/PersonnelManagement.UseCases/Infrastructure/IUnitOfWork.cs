namespace PersonnelManagement.UseCases.Infrastructure;

public interface IUnitOfWork
{
    Task Begin();
    Task Commit();
    Task Rollback();
    Task Complete(string userId);
    Task Complete();
}