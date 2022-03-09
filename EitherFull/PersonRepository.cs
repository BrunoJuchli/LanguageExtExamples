using LanguageExt;

namespace EitherFull;

public class PersonRepository
{
    private readonly Cache<PersonId, Person> cache = new();
    private readonly Database database = new();

    public Option<Person> TryGetPersonById(PersonId id)
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

    public Either<AddPersonErrorResult, Person> TryAdd(Person person)
    {
        return database
            .Add(person)
            .Map(person => cache.AddOrUpdate(person.Id, person));
    }
}
