namespace PersonnelManagement.UseCases.Infrastructure.PaginationUtilities;

public class PageResult<T> : IPageResult<T>
{
    public IEnumerable<T> Elements { get; }
    public int TotalElements { get; }

    public PageResult(IEnumerable<T> elements, int totalElements)
    {
        Elements = elements;
        TotalElements = totalElements;
    }
}