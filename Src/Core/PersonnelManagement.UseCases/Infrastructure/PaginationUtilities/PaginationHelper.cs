namespace PersonnelManagement.UseCases.Infrastructure.PaginationUtilities;

public static class PaginationHelper
{
    public static IQueryable<T> Page<T>(
        this IQueryable<T> source, Pagination? pagination)
    {
        var query = source;

        if (pagination != null && pagination.PageNumber > 1)
        {
            query = query.Skip(
                (pagination.PageNumber - 1) * pagination.PageSize);
        }

        if (pagination != null)
            query = query.Take(pagination.PageSize);

        return query;
    }
}