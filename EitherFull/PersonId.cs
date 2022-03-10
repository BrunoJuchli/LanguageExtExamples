using LanguageExt;

namespace EitherFull;

public record class PersonId
{
    private static readonly RangeInclusive Range =
        RangeInclusive.Create(5, 100);

    private PersonId(int Value)
    {
        this.Value = Value;
    }

    public int Value { get; }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static Either<PersonIdValidationError, PersonId> TryCreate(int value)
    {
        return Range.IsInRange(value)
            ? new PersonId(value)
            : new PersonIdValidationError.OutOfRange(
                value,
                Range);
    }
}
