using LanguageExt;

namespace EitherFull;

public record class NameValidationError
{
    private NameValidationError() { }

    public sealed record class Length(
            string Name,
            RangeInclusive Range)
        : NameValidationError;

    public sealed record class ForbiddenCharacter(
            string Name,
            Seq<(int index, char character)> Violations,
            Seq<char> ForbiddenCharacters)
        : NameValidationError;
}

