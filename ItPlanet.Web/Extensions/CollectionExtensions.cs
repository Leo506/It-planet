namespace ItPlanet.Web.Extensions;

public static class CollectionExtensions
{
    public static bool HasDuplicates<T>(this IEnumerable<T> collection)
    {
        var list = collection.ToList();
        return list.ToHashSet().Count != list.Count();
    }

    public static (T? previous, T? current, T? next) GetPreviousAndNextElements<T>(this IEnumerable<T> collection,
        Func<T, bool> predicate)
    {
        var previous = default(T);

        using var enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var item = enumerator.Current;

            if (predicate(item))
            {
                var next = enumerator.MoveNext() ? enumerator.Current : default;
                return (previous, item, next);
            }

            previous = item;
        }

        return (default, default, default);
    }
}