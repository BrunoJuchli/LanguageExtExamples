using LanguageExt;

namespace EitherFull;

public record class PersonId
{
    private static readonly RangeInclusive Range =
        RangeInclusive
            .TryCreate(
                5,
                100)
            .IfLeft(() =>
                throw new Exception(
                    $"Programming error defining {nameof(PersonId)}.{nameof(Range)}"));

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
