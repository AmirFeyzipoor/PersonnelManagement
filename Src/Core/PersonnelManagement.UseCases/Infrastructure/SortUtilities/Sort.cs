using System.Reflection;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities.Exceptions;

namespace PersonnelManagement.UseCases.Infrastructure.SortUtilities;

public class Sort<T> : ISort<T>
{
    private readonly Dictionary<MemberInfo, SortDirection> _orders;

    public (MemberInfo Property, SortDirection Direction)[] OrdersToArray()
    {
        return _orders.Select(_ => (_.Key, _.Value)).ToArray();
    }

    public Dictionary<MemberInfo, SortDirection> GetOrders()
    {
        return _orders;
    }
    
    private Sort(IEnumerable<KeyValuePair<MemberInfo, SortDirection>> orders)
    {
        _orders = orders.GroupBy(
            _ => _.Key).ToDictionary(_ => _.Key, _ => _.Last().Value);
    }

    public ISort<T> And(ISort<T> sort)
    {
        return new Sort<T>(_orders.Concat(sort.GetOrders()));
    }

    public static Sort<T> By(string propertyName, SortDirection direction)
    {
        var property = ResolveProperty(propertyName);

        if (property == null)
            throw new InvalidSortQueryParamException();

        return new Sort<T>(new[]
        {
            new KeyValuePair<MemberInfo, SortDirection>(property, direction)
        });
    }

    private static MemberInfo ResolveProperty(string propertyName)
    {
        var type = typeof(T);
        var searchFlags = BindingFlags.Public |
                          BindingFlags.Instance |
                          BindingFlags.IgnoreCase;
        return (MemberInfo) type.GetProperty(
            propertyName,
            searchFlags) ?? type.GetField(propertyName, searchFlags);
    }
}