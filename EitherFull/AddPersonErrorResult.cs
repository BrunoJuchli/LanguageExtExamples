namespace EitherFull;

public record AddPersonErrorResult
{
    private AddPersonErrorResult(Person person)
    {
        Person = person;
    }

    public Person Person { get; }

    public sealed record DuplicateId
        : AddPersonErrorResult
    {
        public DuplicateId(Person person)
            : base(person)
        {
        }
    }

    public sealed record DuplicateName
        : AddPersonErrorResult
    {
        public DuplicateName(Person person)
            : base(person)
        {
        }
    }
}
