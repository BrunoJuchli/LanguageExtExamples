using LanguageExt;
using System.Collections.Concurrent;

using static LanguageExt.Prelude;

namespace EitherFull;

public class Cache<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> dictionary = new();

    public TValue AddOrUpdate(TKey key, TValue value)
    {
        return dictionary.AddOrUpdate(
            key,
            value,
            (key, existingValue) => value);
    }

    public Option<TValue> Get(TKey key)
    {
        if (dictionary.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        return None;
    }
}
