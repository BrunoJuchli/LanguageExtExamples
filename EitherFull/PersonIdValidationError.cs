namespace EitherFull;

public record class PersonIdValidationError
{
    private PersonIdValidationError() { }

    public sealed record class OutOfRange(
            int Id,
            RangeInclusive Range)
        : PersonIdValidationError;
}
