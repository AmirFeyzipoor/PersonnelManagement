namespace PersonnelManagement.UseCases.Infrastructure.SortUtilities;

public class UriSortParser
{
    public ISort<T> Parse<T>(string expression)
    {
        var sortExprs = expression.Trim().Split(',');
        var sorts = sortExprs.Select(ExpressionToSort<T>);
        return sorts.Aggregate(
            (ISort<T>)null,
            (previous, current) => 
                previous != null ? previous.And(current) : current);
    }

    private ISort<T> ExpressionToSort<T>(string expression)
    {
        var trimmedExpression = expression.Trim();
        var prefix = trimmedExpression[0];
        var propertyName = trimmedExpression.TrimStart('+', '-');

        if (prefix == '-')
            return Sort<T>.By(propertyName, SortDirection.Descending);

        return Sort<T>.By(propertyName, SortDirection.Ascending);
    }
}