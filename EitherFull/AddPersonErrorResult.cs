namespace EitherFull;

public record class AddPersonErrorResult
{
    private AddPersonErrorResult(Person person)
    {
        Person = person;
    }

    public Person Person { get; }

    public sealed record class DuplicateId
        : AddPersonErrorResult
    {
        public DuplicateId(Person person)
            : base(person)
        {
        }
    }

    public sealed record class DuplicateName
        : AddPersonErrorResult
    {
        public DuplicateName(Person person)
            : base(person)
        {
        }
    }
}
