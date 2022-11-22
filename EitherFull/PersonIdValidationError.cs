namespace EitherFull;

public record PersonIdValidationError
{
    private PersonIdValidationError() { }

    public sealed record OutOfRange(
            int Id,
            RangeInclusive Range)
        : PersonIdValidationError;
}
