using LanguageExt;

using static LanguageExt.Prelude;

namespace EitherFull;

public record class Name
{
    private static readonly RangeInclusive LengthRange =
        RangeInclusive
            .TryCreate(3, 10)
            .IfLeft(() =>
                throw new Exception(
                    $"Programming error defining {nameof(Name)}.{nameof(LengthRange)}"));

    private static readonly Seq<char> ForbiddenCharacters
        = Seq(' ', 'F', '!');

    private readonly Lazy<string> normalized;

    private Name(string value)
    {
        Value = value;
        normalized = new Lazy<string>(() => value.ToUpperInvariant());
    }

    public string Value { get; }

    public virtual bool Equals(Name? other)
    {
        return other != null &&
            normalized.Value == other.normalized.Value;
    }

    public override int GetHashCode()
    {
        return normalized.Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static Either<NameValidationError, Name> TryCreate(string value)
    {
        return ValidateLength(value)
            .IfLeft(() => ValidateForbiddenCharacters(value))
            .ToEither(() => new Name(value))
            .Swap();
    }

    private static Option<NameValidationError> ValidateLength(string value)
    {
        if (!LengthRange.IsInRange(value.Length))
        {
            return new NameValidationError.Length(
                value,
                LengthRange);
        }

        return None;
    }

    private static Option<NameValidationError> ValidateForbiddenCharacters(string value)
    {
        Seq<(int index, char character)> forbiddenCharactersViolations = FindAllOccurencesOfCharacters(
            value,
            ForbiddenCharacters);

        if (forbiddenCharactersViolations.Any())
        {
            return new NameValidationError.ForbiddenCharacter(
                value,
                forbiddenCharactersViolations,
                ForbiddenCharacters);
        }

        return None;
    }

    private static Seq<(int index, char character)> FindAllOccurencesOfCharacters(
        string value,
        Seq<char> characters)
    {
        char[] upperCaseCharacters = characters
            .Select(x => char.ToUpperInvariant(x))
            .ToArray();

        return value
            .Select(
                (c, index) =>
                    upperCaseCharacters
                        .FirstOrNone(other => other == char.ToUpperInvariant(c))
                        .Map(match => (index, match)))
            .SelectMany(x => x)
            .ToSeq();
    }
}

