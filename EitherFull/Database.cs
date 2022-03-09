using LanguageExt;
using System.Collections;

using static LanguageExt.Prelude;

namespace EitherFull;

public class Database
{
    private readonly Dictionary<PersonId, Person> data;
    private readonly object syncRoot;

    public Database()
    {
        data = new();
        syncRoot = ((ICollection)data).SyncRoot;
    }

    public Option<Person> GetPersonById(PersonId id)
    {
        lock (syncRoot)
        {
            return data.TryGetValue(id, out Person? person)
                ? person
                : None;
        }
    }

    public Either<AddPersonErrorResult, Person> Add(Person person)
    {
        lock (syncRoot)
        {
            if (data.Values.Any(otherPerson => person.Name == otherPerson.Name))
            {
                return new AddPersonErrorResult.DuplicateName(person);
            }

            return data.TryAdd(person.Id, person)
                ? person
                : new AddPersonErrorResult.DuplicateId(person);
        }
    }
}
