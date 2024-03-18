namespace PersonnelManagement.UseCases.Infrastructure.PaginationUtilities;

public class Pagination
{
    public int PageNumber { get; }
    public int PageSize { get; }

    private Pagination(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static Pagination Of(int pageNumber, int pageSize)
    {
        return new Pagination(pageNumber, pageSize);
    }
}