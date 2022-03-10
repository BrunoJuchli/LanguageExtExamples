using LanguageExt;

namespace EitherFull;

public record struct RangeInclusive
{
    private RangeInclusive(
        int Minimum,
        int Maximum)
    {
        this.Minimum = Minimum;
        this.Maximum = Maximum;
    }

    public int Minimum { get; }
    public int Maximum { get; }

    public bool IsInRange(int number)
    {
        return !(number < Minimum || number > Maximum);
    }

    public static Either<string, RangeInclusive> TryCreate(
        int minimum,
        int maximum)
    {
        if (minimum > maximum)
        {
            return $"The minimum {minimum} is larger than the maximum {maximum}";
        }

        return new RangeInclusive(
            minimum,
            maximum);
    }

    public static RangeInclusive Create(
        int minimum,
        int maximum)
    {
        return TryCreate(minimum, maximum)
            .IfLeft(error =>
                throw new ArgumentOutOfRangeException(
                    error));
    }
}
