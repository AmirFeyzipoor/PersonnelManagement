namespace PersonnelManagement.IntegrationTest.Infrastructure;

public static class EnumerableHelper
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            action(enumerator.Current);
        }
    }
}