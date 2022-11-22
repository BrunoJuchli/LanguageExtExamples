using LanguageExt;
using System.Collections.Concurrent;

using static LanguageExt.Prelude;

namespace Option;

public record PersonId(
    Guid Id)
{
}

public record Person(
    PersonId Id,
    string Name)
{
}

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

    public Option<TValue> TryGet(TKey key)
    {
        if (dictionary.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        return None;
    }
}

public class Database
{
    private readonly ConcurrentDictionary<PersonId, Person> data = new();

    public Option<Person> TryGetPersonById(PersonId id)
    {
        return data.TryGetValue(id, out Person? person)
            ? person
            : None;
    }

    public Option<Person> TryAdd(Person person)
    {
        return data.TryAdd(person.Id, person)
            ? person
            : None;
    }
}

public class PersonRepository // Compare this with null ref implementation
{
    private readonly Cache<PersonId, Person> cache = new();
    private readonly Database database = new();

    public Option<Person> TryGetPersonById(PersonId id)
    {
        return cache.TryGet(id)
            || TryGetPersonFromDatabaseAndStoreInCache(id);
    }

    private Option<Person> TryGetPersonFromDatabaseAndStoreInCache(PersonId id)
    {
        return database
            .TryGetPersonById(id)
            .Map(person => cache.AddOrUpdate(id, person)); // not using if's
    }

    public Option<Person> TryAdd(Person person)
    {
        return database
            .TryAdd(person)
            .Map(person => cache.AddOrUpdate(person.Id, person)); // not using if's -- there's other methods to operator on the state as well

        //Option<Person> foo = None;

        // foo.Bind(person => GetWife(person)); // Bind returns another option which can be of a different type

        // int nameLength = foo.Match( // match handles both cases and (optionally) returns a value of the same type for both.
        //     person => person.Name.Length,
        //     () => 0)
        //// Alternative:
        // int nameLength2 = foo
        //     .Map(_ => person.Name.Length)
        //     .IfNone(() => 0);

        // works well for config stuff:
        // config.Port.IfNone(() => 6300)
    }
}

public class Program
{
    // putting it toghether in a simple example
    public static void Main()
    {
        PersonRepository personRepo = new();

        PersonId personId = new(Guid.NewGuid());

        Person person = personRepo
            .TryGetPersonById(personId)
            .IfNone(() => personRepo
                .TryAdd(new Person(personId, "Fido Option"))
                .IfNone(() =>
                    throw new Exception(
                        $"Failed to add person with id {personId}")));

        Console.WriteLine(person.Name);

        Console.ReadKey();
    }
}