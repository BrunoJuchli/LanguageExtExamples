﻿using LanguageExt;
using System.Collections;
using System.Collections.Concurrent;

using static LanguageExt.Prelude;

namespace Either;

public record class PersonId(
    Guid Id)
{
}

public record class Person(
    PersonId Id,
    string Name)
{
}

public record class AddPersonErrorResult
{
    private AddPersonErrorResult()
    {
    }

    public sealed record class DuplicateId(
            PersonId Id)
        : AddPersonErrorResult;

    public sealed record class DuplicateName(
            string Name)
        : AddPersonErrorResult;
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
            if (data.Values.Any(otherPerson => HaveSameName(person, otherPerson)))
            {
                return new AddPersonErrorResult.DuplicateName(person.Name);
            }

            return data.TryAdd(person.Id, person)
                ? person
                : new AddPersonErrorResult.DuplicateId(person.Id);
        }
    }

    private static bool HaveSameName(Person p1, Person p2)
    {
        return 0 == string.Compare(
                   p1.Name,
                   p2.Name,
                   StringComparison.OrdinalIgnoreCase);
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

    public Either<AddPersonErrorResult, Person> Add(Person person)
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
                .Add(new Person(personId, "Fido Either"))
                .IfLeft(error =>
                    throw new Exception(
                        CreateErrorMessageFor(error))));

        Console.WriteLine(person.Name);

        Console.ReadKey();
    }

    private static string CreateErrorMessageFor(AddPersonErrorResult errorResult)
    {
        return errorResult switch
        {
            AddPersonErrorResult.DuplicateName duplicateName
                => $"The name '{duplicateName.Name}' is already taken.",
            AddPersonErrorResult.DuplicateId duplicateId
                => $"The id '{duplicateId.Id}' is already taken.",
            _
                => throw new ArgumentOutOfRangeException(
                    $"unfortunately c# doesn't know sum types so the compiler doesn't prevent us from forgetting cases")
        };
    }
}