using LanguageExt;

using static LanguageExt.Prelude;

namespace System.Linq;

public static class EnumerableExtensions
{
    public static Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source)
        where TSource : notnull
    {
        return source
            .FirstOrNone(
                x => true);
    }

    public static Option<TSource> FirstOrNone<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
        where TSource : notnull
    {
        return source
            .Where(predicate)
            .Select(e => Some(e))
            .FirstOrDefault();
    }
}