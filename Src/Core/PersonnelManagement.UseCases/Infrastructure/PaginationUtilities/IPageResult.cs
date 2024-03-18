namespace PersonnelManagement.UseCases.Infrastructure.PaginationUtilities;

public interface IPageResult<T>
{
    public IEnumerable<T> Elements { get; }
    public int TotalElements { get; }
}