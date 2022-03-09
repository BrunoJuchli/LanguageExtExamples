using System.Text;

namespace EitherFull;

public class Program
{
    private readonly PersonRepository repository = new();

    /// <summary>
    /// Adds sample data
    /// </summary>
    public Program()
    {
        repository.TryAdd(
            new Person(
                PersonId.TryCreate(5).IfLeft(() => throw new Exception("Unexpected")),
                Name.TryCreate("Hans").IfLeft(() => throw new Exception("Unexpected"))));
    }

    public static void Main()
    {
        Program p = new();

        while (true)
        {
            Console.WriteLine("Choose what to do next by pressing any of the following characters:");
            Console.WriteLine(" f - find existing user");
            Console.WriteLine(" a - add new user");
            Console.WriteLine(" q - quit");
            ConsoleKeyInfo input = Console.ReadKey();
            Console.WriteLine();

            switch (char.ToLowerInvariant(input.KeyChar))
            {
                case 'f':
                    p.FindPerson();
                    break;
                case 'a':
                    p.AddNewPerson();
                    break;
                case 'q':
                    return;
            }
        }
    }

    private void FindPerson()
    {
        PersonId id = AskForId();

        string consoleOutput = repository
            .TryGetPersonById(id)
            .Match(
                person => $"Record found: {person}",
                () => "No record found");

        Console.WriteLine(consoleOutput);
    }

    private void AddNewPerson()
    {
        Person addedPerson = AddNewPerson(
            new(
                AskForId(),
                AskForName()));

        Console.WriteLine(
            $"Added new {addedPerson}");
    }

    private Person AddNewPerson(
        Person toAdd)
    {
        return repository
            .TryAdd(toAdd)
            .IfLeft(error =>
                AddNewPerson(
                    AskToFixError(
                        error)));
    }

    private Person AskToFixError(
        AddPersonErrorResult error)
    {
        (string errorMessage, Func<Person> resolution) result = error switch
        {
            AddPersonErrorResult.DuplicateId duplicateId
                => ($"The id {duplicateId.Person.Id} already exists.", () => duplicateId.Person with { Id = AskForId() }),
            AddPersonErrorResult.DuplicateName duplicateName
                => ($"User with name '{duplicateName.Person.Name}' already exists.", () => duplicateName.Person with { Name = AskForName() }),
            _
                => throw new ArgumentOutOfRangeException(
                    $"unfortunately c# doesn't know sum types so the compiler doesn't prevent us from forgetting cases")
        };

        Console.WriteLine(result.errorMessage);
        return result.resolution();
    }


    private static PersonId AskForId()
    {
        Console.WriteLine("Please enter the id");
        string? input = Console.ReadLine();
        if (input == null)
        {
            Console.WriteLine("The id must not be null or empty");
            return AskForId();
        }

        if (!int.TryParse(input, out int number))
        {
            Console.WriteLine("The id must be an integer.");
            return AskForId();
        }

        return PersonId
            .TryCreate(number)
            .IfLeft(error =>
                {
                    Console.WriteLine(
                        CreateErrorMessageFor(
                            error));
                    return AskForId();
                });
    }

    private static string CreateErrorMessageFor(PersonIdValidationError error)
    {
        return error switch
        {
            PersonIdValidationError.OutOfRange outOfRange
                => $"{outOfRange.Id} is outside of the acceptable Range. The id must be at least {outOfRange.Range.Minimum} and not more than {outOfRange.Range.Maximum}",
            _
                => throw new ArgumentOutOfRangeException(
                    $"unfortunately c# doesn't know sum types so the compiler doesn't prevent us from forgetting cases")
        };
    }

    private static Name AskForName()
    {
        Console.WriteLine("Please enter the desired name");

        string? input = Console.ReadLine();
        if (input == null)
        {
            Console.WriteLine("The name must not be null");
            return AskForName();
        }

        return Name
            .TryCreate(input!)
            .IfLeft(
                error =>
                {
                    Console.WriteLine(
                        CreateErrorMessageFor(
                            error));
                    return AskForName();
                });
    }

    private static string CreateErrorMessageFor(NameValidationError nameValidationError)
    {
        return nameValidationError switch
        {
            NameValidationError.Length length
                => $"The name '{length.Name}' is {length.Name.Length} long but must not be less than {length.Range.Minimum} and not more than {length.Range.Maximum} characters",
            NameValidationError.ForbiddenCharacter forbiddenCharacter
                => CreateErrorMessageFor(forbiddenCharacter),
            _
                => throw new ArgumentOutOfRangeException(
                    $"unfortunately c# doesn't know sum types so the compiler doesn't prevent us from forgetting cases")
        };
    }

    private static string CreateErrorMessageFor(NameValidationError.ForbiddenCharacter forbiddenCharacterError)
    {
        StringBuilder? sb = new();

        sb.AppendLine(
            $"The name '{forbiddenCharacterError.Name}' contains the following forbidden characters:");

        foreach ((int index, char character) forbiddenCharacter in forbiddenCharacterError.Violations)
        {
            sb.AppendLine(
                $" - '{forbiddenCharacter.character}' at index {forbiddenCharacter.index}");
        }

        return sb.ToString();
    }
}