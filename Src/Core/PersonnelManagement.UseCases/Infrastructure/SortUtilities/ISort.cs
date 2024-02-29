using System.Reflection;

namespace PersonnelManagement.UseCases.Infrastructure.SortUtilities;

public interface ISort<T>
{
    ISort<T> And(ISort<T> sort);
    (MemberInfo Property, SortDirection Direction)[] OrdersToArray();
    Dictionary<MemberInfo, SortDirection> GetOrders();
}

public enum SortDirection
{
    Ascending,
    Descending
}