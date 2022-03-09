using LanguageExt;
using System.Collections.Concurrent;

using static LanguageExt.Prelude;

namespace Option;

public record class PersonId(
    Guid Id)
{
}

public record class Person(
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

    public Option<TValue> Get(TKey key)
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

    public Option<Person> GetPersonById(PersonId id)
    {
        return data.TryGetValue(id, out Person? person)
            ? person
            : None;
    }

    public Option<Person> Add(Person person)
    {
        return data.TryAdd(person.Id, person)
            ? person
            : None;
    }
}

public class PersonRepository
{
    private readonly Cache<PersonId, Person> cache = new();
    private readonly Database database = new();

    public Option<Person> GetPersonById(PersonId id)
    {
        return cache.Get(id)
            || GetPersonFromDatabaseAndStoreInCache(id);
    }

    private Option<Person> GetPersonFromDatabaseAndStoreInCache(PersonId id)
    {
        return database
            .GetPersonById(id)
            .Map(person => cache.AddOrUpdate(id, person));
    }

    public Option<Person> Add(Person person)
    {
        return database
            .Add(person)
            .Map(person => cache.AddOrUpdate(person.Id, person));
    }
}

public class Program
{
    public static void Main()
    {
        PersonRepository personRepo = new();

        PersonId personId = new(Guid.NewGuid());

        Person person = personRepo
            .GetPersonById(personId)
            .IfNone(() => personRepo
                .Add(new Person(personId, "Fido Option"))
                .IfNone(() =>
                    throw new Exception(
                        $"Failed to add person with id {personId}")));

        Console.WriteLine(person.Name);

        Console.ReadKey();
    }
}