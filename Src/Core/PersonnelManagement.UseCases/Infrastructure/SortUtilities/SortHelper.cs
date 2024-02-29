using System.Linq.Expressions;
using System.Reflection;

namespace PersonnelManagement.UseCases.Infrastructure.SortUtilities
{
    public static class SortHelper
    {
        private static readonly Lazy<MethodInfo> _queryableOrderByMethod;

        private static readonly Lazy<MethodInfo> _queryableThenByMethod;

        private static readonly Lazy<MethodInfo> _queryableOrderByDescendingMethod;

        private static readonly Lazy<MethodInfo> _queryableThenByDescendingMethod;

        static SortHelper()
        {
            _queryableThenByDescendingMethod = new Lazy<MethodInfo>(
                ()
                => ResolveQueryableMethod(nameof(Queryable.ThenByDescending),
                parameterCount: 2));

            _queryableOrderByDescendingMethod = new Lazy<MethodInfo>(
                ()
                => ResolveQueryableMethod(nameof(Queryable.OrderByDescending),
                parameterCount: 2));

            _queryableThenByMethod = new Lazy<MethodInfo>(
                ()
                => ResolveQueryableMethod(nameof(Queryable.ThenBy),
                parameterCount: 2));

            _queryableOrderByMethod = new Lazy<MethodInfo>(
                ()
                => ResolveQueryableMethod(nameof(Queryable.OrderBy),
                parameterCount: 2));
        }

        private static MethodInfo ResolveQueryableMethod(
            string name,
            int parameterCount)
        {
            var classType = typeof(Queryable);
            var searchFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod;
            return classType
                .GetMember(name, searchFlags)
                .OfType<MethodInfo>()
                .Single(_ => _.GetParameters().Length == parameterCount);
        }

        public static IQueryable<T> Sort<T>(
            this IQueryable<T> source,
            ISort<T>? sort)
        {
            var query = source;

            var sortOrders = sort!.OrdersToArray();
            for (var propertyIndex = 0; 
                 propertyIndex < sortOrders.Length;
                 propertyIndex++)
            {
                var (property, direction) = sortOrders[propertyIndex];
                var secondLevel = propertyIndex > 0;
                query = SortSingleProperty(
                                query,
                                property,
                                direction,
                                secondLevel);
            }

            return query;
        }

        private static IQueryable<T> SortSingleProperty<T>(
            IQueryable<T> query,
            MemberInfo member,
            SortDirection sortDirection,
            bool isSecondLevel)
        {
            var sourceType = typeof(T);
            var parameter = Expression.Parameter(sourceType, name: "_");
            var propertyExpr = Expression.MakeMemberAccess(parameter, member);
            var lambdaPropertyExpr = Expression.Lambda(propertyExpr, parameter);

            var sortMethod = (sortDirection, isSecondLevel) switch
            {
                (SortDirection.Ascending, Item2: false) 
                    => _queryableOrderByMethod.Value,
                (SortDirection.Ascending, Item2: true)
                    => _queryableThenByMethod.Value,
                (SortDirection.Descending, Item2: false) 
                    => _queryableOrderByDescendingMethod.Value,
                (SortDirection.Descending, Item2: true) 
                    => _queryableThenByDescendingMethod.Value,
            };

            query = (IQueryable<T>)sortMethod
                .MakeGenericMethod(sourceType, GetMemberType(member))
                .Invoke(
                    null, 
                    new object[] { query, lambdaPropertyExpr });

            return query;
        }

        private static Type GetMemberType(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo property => property.PropertyType,
                FieldInfo field => field.FieldType,

                _ => throw new ArgumentException(
                $"member '{member.DeclaringType.Name}.{member.Name}'" +
                $" not found.")
            };
        }
    }
}
