using System.Collections.Concurrent;

namespace NullRef;

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

    public bool TryGet(TKey key, out TValue? value)
    {
        return dictionary.TryGetValue(key, out value);
    }
}

public class Database
{
    private readonly ConcurrentDictionary<PersonId, Person> data = new();

    public bool TryGetPersonById(PersonId id, out Person? person)
    {
        return data.TryGetValue(id, out person);
    }

    public bool TryAdd(Person person)
    {
        return data.TryAdd(person.Id, person);
    }
}

public class PersonRepository
{
    private readonly Cache<PersonId, Person> cache = new();
    private readonly Database database = new();

    public bool TryGetPersonById(PersonId id, out Person? person)
    {
        return cache.TryGet(id, out person)
            || GetPersonFromDatabaseAndStoreInCache(id, out person);
    }

    private bool GetPersonFromDatabaseAndStoreInCache(PersonId id, out Person? person)
    {
        if (database.TryGetPersonById(id, out person))
        {
            person = cache.AddOrUpdate(id, person!); // <-- need to use !. Can do because I know it's not null since we're in the true branch...
            return true;
        }

        return false;
    }

    public bool TryAdd(Person person)
    {
        if (database.TryAdd(person))
        {
            cache.AddOrUpdate(person.Id, person);
            return true;
        }

        return false; // the case is not automatically being forwarded, need to explicitly state again. Possible mistake.
    }
}

public class Program
{
    public static void Main()
    {
        PersonRepository personRepo = new();

        PersonId personId = new(Guid.NewGuid());

        Person person;

        if (personRepo.TryGetPersonById(personId, out Person? existingPerson))
        {
            person = existingPerson!; // the ! again
        }
        else
        {
            person = new Person(personId, "Fido nullref"); // notice how the person is available before we check whether adding is successful.
            if (!personRepo.TryAdd(person))
            {
                throw new Exception(
                    $"Failed to add person with id {personId}");
            }
        }

        Console.WriteLine(person.Name);

        Console.ReadKey();
    }
}